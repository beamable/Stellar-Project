"use client"

import { useEffect, useMemo, useRef, useState } from "react"
import type { Beam } from "beamable-sdk"
import type { BallType, BallTypeConfig } from "@/components/Game/types"
import { BALL_TYPES, DEFAULT_BALL_TYPE_MAP } from "@/components/Game/ballTypes"
import type { BallContent } from "@/lib/beamContent"
import { resolveBallContent } from "@/lib/beamContent"
import getBeam from "@/lib/beam"
import { fetchInventory } from "@/lib/beamInventory"
import { debugLog } from "@/lib/debugLog"
import { waitForMintingDelayOffset } from "@/lib/mintingDelay"

type UseBallLoadoutResult = {
  ballTypes: BallTypeConfig[]
  ballTypeMap: Record<BallType, BallTypeConfig>
  ownedBallTypes: BallType[]
  ownedBallInventory: OwnedBallInstance[]
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
  const aliasMap: Record<string, BallType> = {
    fireball: "fire",
    fire_ball: "fire",
    fire: "fire",
    defaultball: "normal",
    default_ball: "normal",
    default: "normal",
    multishot: "multishot",
    multi_shot: "multishot",
    laserball: "laser",
    laser_ball: "laser",
    laser: "laser",
    normalball: "normal",
    normal_ball: "normal",
    normal: "normal",
  }
  const mapped = aliasMap[slug]
  if (mapped) return mapped
  const match = ALLOWED_TYPES.find((t) => t === slug)
  return match ?? null
}

const mapContentToConfigs = (content: BallContent[]): BallTypeConfig[] => {
  const defaults = DEFAULT_BALL_TYPE_MAP
  const mapped: BallTypeConfig[] = []
  content.forEach((entry) => {
    const raw = entry as any
    const derivedType = deriveBallType(entry.id, raw?.type ?? raw?.customProperties?.type ?? raw?.properties?.type)
    if (!derivedType) return
    const base = defaults[derivedType] ?? BALL_TYPES[0]
    const props = raw?.customProperties ?? raw?.properties ?? {}
    const entryName = typeof raw?.name === "string" ? raw.name : undefined
    const entryDescription = typeof raw?.description === "string" ? raw.description : undefined
    mapped.push({
      type: derivedType,
      name: entryName ?? base.name,
      description: entryDescription ?? base.description,
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

  const pushInstance = (type: BallType | null, contentId?: any, instanceId?: any) => {
    if (!type) return
    owned.push({ type, instanceId, contentId })
  }

  const visitItem = (item: any) => {
    if (typeof item === "string") {
      const typeFromString = deriveBallType(item)
      if (typeFromString) {
        pushInstance(typeFromString, item, item)
      }
      return
    }

    // If this looks like a grouped item { id: contentId, items: [...] }, use child ids as instanceIds.
    if (Array.isArray(item?.items) && typeof item?.id === "string") {
      const parentContentId =
        item.id ??
        item.contentId ??
        item.content?.id ??
        item.properties?.contentId ??
        item.content?.properties?.contentId
      const parentTypeCandidate =
        item?.type ??
        item?.customType ??
        item?.properties?.type ??
        item?.content?.type ??
        item?.content?.properties?.type
      const parentType = deriveBallType(parentContentId, parentTypeCandidate)
      item.items.forEach((child: any) => {
        const childInstanceId = child?.instanceId ?? child?.id
        const childTypeCandidate =
          child?.type ??
          child?.customType ??
          child?.properties?.type ??
          child?.content?.type ??
          child?.content?.properties?.type ??
          parentTypeCandidate
        const childType = deriveBallType(parentContentId, childTypeCandidate) ?? parentType
        pushInstance(childType, parentContentId, childInstanceId)
      })
    }

    const contentId =
      item?.contentId ??
      item?.id ??
      item?.content?.id ??
      item?.properties?.contentId ??
      item?.content?.properties?.contentId
    const instanceId = item?.instanceId ?? item?.id
    const typeCandidate =
      item?.type ??
      item?.customType ??
      item?.properties?.type ??
      item?.content?.type ??
      item?.content?.properties?.type
    const type = deriveBallType(contentId, typeCandidate)
    pushInstance(type, contentId, instanceId)
  }

  const visited = new Set<any>()
  const walk = (val: any) => {
    if (!val || typeof val !== "object") return
    if (visited.has(val)) return
    visited.add(val)

    const isCandidate =
      typeof val === "object" &&
      (typeof (val as any).contentId === "string" ||
        typeof (val as any).id === "string" ||
        typeof (val as any).content?.id === "string")
    if (isCandidate) {
      visitItem(val)
    }

    if (Array.isArray(val)) {
      val.forEach(walk)
    } else {
      Object.values(val).forEach(walk)
    }
  }

  // Walk the full inventory object to catch nested shapes we may not anticipate.
  walk(inventory)

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

const dedupeByType = (instances: OwnedBallInstance[]): OwnedBallInstance[] => {
  const map = new Map<BallType, OwnedBallInstance>()
  instances.forEach((entry) => {
    if (!entry.type) return
    if (!map.has(entry.type)) {
      map.set(entry.type, entry)
    }
  })
  return Array.from(map.values())
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
    debugLog("[BallLoadout] AddItem already in-flight; skipping duplicate grant.")
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
    const payload = { itemContentId: targetContentId, properties: {} as Record<string, string> }
    debugLog("[BallLoadout] Granting default ball content with payload:", payload)
    addInFlightRef.current = true
    try {
      await (beam as any).stellarFederationClient.addItem(payload)
      await waitForMintingDelayOffset()
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
  const [ownedBallInventory, setOwnedBallInventory] = useState<OwnedBallInstance[]>([])
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
      setOwnedBallTypes(["normal"])
      setBallTypes(BALL_TYPES)
      return
    }

    let cancelled = false
    setLoading(true)
    ;(async () => {
      try {
        debugLog("[BallLoadout] Starting ball loadout fetch...")
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
        debugLog("[BallLoadout] Fetching inventory for owned ball items...")
        const inventory = await fetchInventory(beam)
        let ownedBallInstances = extractOwnedBallInstances(inventory)
        const dedupedByType = dedupeByType(ownedBallInstances)
        setOwnedBallInventory(dedupedByType)

        const granted = await ensureDefaultBallIfNeeded({
          beam,
          ownedBallInstances: dedupedByType,
          availableBallContentIds: ballContentIds,
          addInFlightRef,
        })
        if (granted) {
          const refreshed = await fetchInventory(beam)
          ownedBallInstances = extractOwnedBallInstances(refreshed)
          const refreshedDeduped = dedupeByType(ownedBallInstances)
          setOwnedBallInventory(refreshedDeduped)
          debugLog("[BallLoadout] Cached owned ball inventory after grant (deduped):", refreshedDeduped)
        }

        const ownedTypes = dedupedByType.map((entry) => entry.type).filter((v): v is BallType => Boolean(v))
        const uniqueOwned = Array.from(new Set(ownedTypes))

        if (!cancelled) {
          setOwnedBallTypes((prev) => {
            // Preserve previously known balls in case the latest inventory snapshot is incomplete.
            const base = prev && prev.length > 0 ? new Set(prev) : new Set<BallType>(["normal"])
            uniqueOwned.forEach((t) => base.add(t))
            const nextOwned = Array.from(base)
            return nextOwned
          })
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
          debugLog("[BallLoadout] Loadout fetch completed.")
        }
      }
    })()

    return () => {
      cancelled = true
    }
  }, [readyForGame, refreshKey])

  return { ballTypes, ballTypeMap, ownedBallTypes, ownedBallInventory, loading }
}
