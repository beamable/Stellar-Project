import { useCallback, useEffect, useMemo, useState } from "react"
import {
  CAMPAIGN_STAGE_MAP,
  CAMPAIGN_STAGES,
  DEFAULT_STAGE_ID,
  type CampaignStage,
  type MechanicTag,
} from "@/components/Game/campaign"

const STORAGE_KEY = "tower-destroyer-campaign-progress"

type StoredCampaignState = {
  selectedStageId: string
  completedStageIds: string[]
  seenMechanics: MechanicTag[]
  loopCount: number
}

type StageStatus = "locked" | "available" | "completed"

export type StageProgress = {
  stage: CampaignStage
  status: StageStatus
}

const MECHANIC_TAG_LOOKUP: Record<MechanicTag, true> = {
  tower_density: true,
  shielded_blocks: true,
  moving_targets: true,
  wind_gusts: true,
  boss_phase: true,
}

const defaultState: StoredCampaignState = {
  selectedStageId: DEFAULT_STAGE_ID,
  completedStageIds: [],
  seenMechanics: [],
  loopCount: 0,
}

const finalStageId = CAMPAIGN_STAGES[CAMPAIGN_STAGES.length - 1]?.id ?? DEFAULT_STAGE_ID

export default function useCampaignProgress() {
  const [state, setState] = useState<StoredCampaignState>(defaultState)
  const [hydrated, setHydrated] = useState(false)

  useEffect(() => {
    if (typeof window === "undefined") return
    try {
      const raw = window.localStorage.getItem(STORAGE_KEY)
      if (raw) {
        const parsed = JSON.parse(raw) as Partial<StoredCampaignState>
        setState((prev) => ({
          selectedStageId: parsed?.selectedStageId && CAMPAIGN_STAGE_MAP.has(parsed.selectedStageId)
            ? parsed.selectedStageId
            : prev.selectedStageId,
          completedStageIds: Array.isArray(parsed?.completedStageIds)
            ? parsed!.completedStageIds.filter((id): id is string => typeof id === "string")
            : prev.completedStageIds,
          seenMechanics: Array.isArray(parsed?.seenMechanics)
            ? parsed!.seenMechanics.filter((tag): tag is MechanicTag =>
                typeof tag === "string" && Object.hasOwn(MECHANIC_TAG_LOOKUP, tag),
              )
            : prev.seenMechanics,
          loopCount: typeof parsed?.loopCount === "number" ? parsed.loopCount : prev.loopCount,
        }))
      }
    } catch {
      // Ignore malformed stored state
    } finally {
      setHydrated(true)
    }
  }, [])

  useEffect(() => {
    if (!hydrated || typeof window === "undefined") return
    try {
      window.localStorage.setItem(STORAGE_KEY, JSON.stringify(state))
    } catch {
      // ignore persistence failures
    }
  }, [state, hydrated])

  const completedSet = useMemo(() => new Set(state.completedStageIds), [state.completedStageIds])

  const stageProgress: StageProgress[] = useMemo(() => {
    return CAMPAIGN_STAGES.map((stage, index) => {
      if (completedSet.has(stage.id)) {
        return { stage, status: "completed" as StageStatus }
      }
      const unlocked = index === 0 || completedSet.has(CAMPAIGN_STAGES[index - 1]!.id)
      return { stage, status: unlocked ? "available" : "locked" }
    })
  }, [completedSet])

  const firstAvailableEntry = stageProgress.find((entry) => entry.status !== "locked")
  const selectedStage =
    CAMPAIGN_STAGE_MAP.get(state.selectedStageId) ??
    firstAvailableEntry?.stage ??
    CAMPAIGN_STAGES[0] ??
    null

  const selectedEntry =
    selectedStage !== null
      ? stageProgress.find((entry) => entry.stage.id === selectedStage.id)
      : undefined
  const resolvedStage =
    selectedEntry && selectedEntry.status !== "locked"
      ? selectedEntry.stage
      : firstAvailableEntry?.stage ?? selectedStage

  const pendingMechanics =
    resolvedStage?.mechanics.filter((tag) => !state.seenMechanics.includes(tag)) ?? []

  const selectStage = useCallback(
    (stageId: string) => {
      const index = CAMPAIGN_STAGES.findIndex((stage) => stage.id === stageId)
      if (index === -1) return
      if (index > 0) {
        const previousStageId = CAMPAIGN_STAGES[index - 1]!.id
        if (!completedSet.has(previousStageId)) {
          return
        }
      }
      setState((prev) => ({ ...prev, selectedStageId: stageId }))
    },
    [completedSet],
  )

  const markStageComplete = useCallback((stageId: string) => {
    setState((prev) => {
      if (prev.completedStageIds.includes(stageId)) return prev
      const nextCompleted = Array.from(new Set([...prev.completedStageIds, stageId]))
      return { ...prev, completedStageIds: nextCompleted }
    })
  }, [])

  const acknowledgeMechanics = useCallback((tags: MechanicTag[]) => {
    if (!tags.length) return
    setState((prev) => {
      const merged = Array.from(new Set([...prev.seenMechanics, ...tags]))
      if (merged.length === prev.seenMechanics.length) return prev
      return { ...prev, seenMechanics: merged }
    })
  }, [])

  const campaignComplete = stageProgress.every((entry) => entry.status === "completed")

  const startNextLoop = useCallback(() => {
    if (!campaignComplete) return
    setState((prev) => ({
      selectedStageId: DEFAULT_STAGE_ID,
      completedStageIds: [],
      seenMechanics: prev.seenMechanics,
      loopCount: prev.loopCount + 1,
    }))
  }, [campaignComplete])

  return {
    hydrated,
    stageProgress,
    selectedStage: resolvedStage,
    selectStage,
    markStageComplete,
    acknowledgeMechanics,
    pendingMechanics,
    campaignComplete,
    loopCount: state.loopCount,
    startNextLoop,
    completedStageIds: state.completedStageIds,
    finalStageId,
  }
}
