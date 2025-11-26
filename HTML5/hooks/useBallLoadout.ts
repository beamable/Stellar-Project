"use client"

import { useEffect, useMemo, useRef, useState } from "react"
import type { Beam } from "beamable-sdk"
import type { BallType, BallTypeConfig } from "@/components/Game/types"
import { BALL_TYPES, DEFAULT_BALL_TYPE_MAP } from "@/components/Game/ballTypes"
import type { BallContent } from "@/lib/beamContent"
import { resolveBallContent } from "@/lib/beamContent"
import getBeam from "@/lib/beam"
import { fetchInventory } from "@/lib/beamInventory"

type UseBallLoadoutResult = {
  ballTypes: BallTypeConfig[]
  ballTypeMap: Record<BallType, BallTypeConfig>
  ownedBallTypes: BallType[]
  loading: boolean
}

const ALLOWED_TYPES: BallType[] = ["normal", "multishot", "fire", "laser"]
const DEFAULT_BALL_CONTENT_ID = "items.ball.DefaultBall"

const parseSpeed = (value: unknown, fallback: number) => {
  const parsed = typeof value === "string" ? Number.parseFloat(value) : typeof value === "number" ? value : NaN
  return Number.isFinite(parsed) ? parsed : fallback
}

const deriveBallType = (id?: string | null, customType?: string | null): BallType | null => {
  const candidate = (customType ?? id ?? "").toLowerCase()
  const slug = candidate.split(".").pop() ?? candidate
  const match = ALLOWED_TYPES.find((t) => t === slug)
  return match ?? null
}

const mapContentToConfigs = (content: BallContent[]): BallTypeConfig[] => {
  const defaults = DEFAULT_BALL_TYPE_MAP
  const mapped: BallTypeConfig[] = []
  content.forEach((entry) => {
    const derivedType = deriveBallType(entry.id, (entry as any)?.type ?? entry.customProperties?.type)
    if (!derivedType) return
    const base = defaults[derivedType] ?? BALL_TYPES[0]
    const props = entry.customProperties ?? {}
    mapped.push({
      type: derivedType,
      name: entry.name ?? base.name,
      description: entry.description ?? base.description,
      icon: (props.icon as string) ?? base.icon,
      color: (props.color as string) ?? base.color,
      baseSpeedMultiplier: parseSpeed((props.baseSpeedMultiplier as unknown) ?? props.speed, base.baseSpeedMultiplier),
    })
  })
  if (mapped.length === 0) return BALL_TYPES
  const order = ALLOWED_TYPES
  mapped.sort((a, b) => order.indexOf(a.type) - order.indexOf(b.type))
  const seen = new Set<string>()
  return mapped.filter((cfg) => {
    if (seen.has(cfg.type)) return false
    seen.add(cfg.type)
    return true
  })
}

type OwnedBallInstance = { type: BallType; instanceId?: string | number; contentId?: string }

const extractOwnedBallInstances = (inventory: any): OwnedBallInstance[] => {
  const owned: OwnedBallInstance[] = []
  if (!inventory) return owned

  const visitItem = (item: any) => {
    const contentId = item?.contentId ?? item?.id ?? item?.content?.id
    const instanceId = item?.instanceId ?? item?.id
    const type = deriveBallType(contentId)
    if (!type) return
    owned.push({ type, instanceId, contentId })
  }

  const items = inventory.items ?? inventory.inventoryItems ?? inventory
  if (Array.isArray(items)) {
    items.forEach(visitItem)
  } else if (items && typeof items === "object") {
    Object.values(items).forEach((value) => {
      if (Array.isArray(value)) {
        value.forEach(visitItem)
      } else {
        visitItem(value)
      }
    })
  }

  const deduped: OwnedBallInstance[] = []
  const seen = new Set<string>()
  owned.forEach((entry) => {
    const key = `${entry.type}:${entry.instanceId ?? entry.contentId ?? "unknown"}`
    if (seen.has(key)) return
    seen.add(key)
    deduped.push(entry)
  })

  return deduped
}

const ensureDefaultBallIfNeeded = async ({
  beam,
  ownedBallInstances,
  availableBallContentIds,
  addInFlightRef,
}: {
  beam: Beam
  ownedBallInstances: OwnedBallInstance[]
  availableBallContentIds: string[]
  addInFlightRef: React.MutableRefObject<boolean>
}): Promise<string[] | null> => {
  if (ownedBallInstances.length > 0) return null
  if (addInFlightRef.current) {
    console.log("[BallLoadout] AddItem already in-flight; skipping duplicate grant.")
    return null
  }
  const normalizeContentId = (id: string) => {
    if (id.startsWith("items.")) return id
    if (id.startsWith("item.")) return id.replace(/^item\./, "items.")
    return `items.${id}`
  }
  const target =
    availableBallContentIds.find((id) => deriveBallType(id) === "normal") ??
    availableBallContentIds[0] ??
    DEFAULT_BALL_CONTENT_ID
  if (!target) return null
  const targetContentId = normalizeContentId(target)
  if ((beam as any)?.stellarFederationClient?.addItem) {
    const payload = { itemContentId: targetContentId }
    console.log("[BallLoadout] Granting default ball content with payload:", payload)
    addInFlightRef.current = true
    try {
      await (beam as any).stellarFederationClient.addItem(payload)
      return [targetContentId]
    } finally {
      addInFlightRef.current = false
    }
  }
  return null
}

export default function useBallLoadout(readyForGame: boolean, refreshKey = 0): UseBallLoadoutResult {
  const [ballTypes, setBallTypes] = useState<BallTypeConfig[]>(BALL_TYPES)
  const [ownedBallTypes, setOwnedBallTypes] = useState<BallType[]>(["normal"])
  const [loading, setLoading] = useState(false)
  const addInFlightRef = useRef(false)

  const ballTypeMap = useMemo(
    () =>
      ballTypes.reduce((acc, cfg) => {
        acc[cfg.type] = cfg
        return acc
      }, {} as Record<BallType, BallTypeConfig>),
    [ballTypes],
  )

  useEffect(() => {
    if (!readyForGame) {
      console.log("[BallLoadout] Not ready for game yet; resetting to defaults.")
      setOwnedBallTypes(["normal"])
      setBallTypes(BALL_TYPES)
      return
    }

    let cancelled = false
    setLoading(true)
    ;(async () => {
      try {
        console.log("[BallLoadout] Starting ball loadout fetch...")
        const content = await resolveBallContent().catch((err) => {
          console.warn("[BallLoadout] Failed to resolve ball content:", err)
          return [] as BallContent[]
        })
        const configs = mapContentToConfigs(content)
        const ballContentIds = content.map((c) => c.id).filter((id) => typeof id === "string")
        if (!cancelled && configs.length > 0) {
          setBallTypes(configs)
        }

        const beam = await getBeam()
        console.log("[BallLoadout] Fetching inventory for owned ball items...")
        const inventory = await fetchInventory(beam)
        let ownedBallInstances = extractOwnedBallInstances(inventory)

        const granted = await ensureDefaultBallIfNeeded({
          beam,
          ownedBallInstances,
          availableBallContentIds: ballContentIds,
          addInFlightRef,
        })
        if (granted) {
          console.log("[BallLoadout] Granted default ball; refreshing inventory.")
          const refreshed = await fetchInventory(beam)
          ownedBallInstances = extractOwnedBallInstances(refreshed)
        }

        const ownedTypes = ownedBallInstances.map((entry) => entry.type).filter((v): v is BallType => Boolean(v))
        const uniqueOwned = Array.from(new Set(ownedTypes))

        if (!cancelled) {
          setOwnedBallTypes(uniqueOwned.length > 0 ? uniqueOwned : ["normal"])
        }
      } catch (err) {
        console.warn("[BallLoadout] Unable to load ball content/inventory:", err)
        if (!cancelled) {
          setOwnedBallTypes(["normal"])
          setBallTypes(BALL_TYPES)
        }
      } finally {
        if (!cancelled) {
          setLoading(false)
          console.log("[BallLoadout] Loadout fetch completed.")
        }
      }
    })()

    return () => {
      cancelled = true
    }
  }, [readyForGame, refreshKey])

  return { ballTypes, ballTypeMap, ownedBallTypes, loading }
}
