"use client"

import { useCallback, useState } from "react"
import type { CampaignStage } from "@/components/Game/campaign"

type CampaignOverlayOptions = {
  readyForGame: boolean
  activeStage: CampaignStage
  showPlayerInfo: boolean
  setShowPlayerInfo: (value: boolean) => void
  commandDeckSeen: boolean
  setCommandDeckSeen: (value: boolean) => void
}

export type CampaignOverlayState = {
  campaignUnlocked: boolean
  campaignConfirmed: boolean
  shouldShowCampaignOverlay: boolean
  confirmCampaignStage: () => void
  showCommandDeck: () => void
  markCommandDeckSeen: () => void
  setCampaignConfirmed: (value: boolean) => void
  resetCampaignOverlay: () => void
}

export function useCampaignOverlay({
  readyForGame,
  activeStage,
  showPlayerInfo,
  setShowPlayerInfo,
  commandDeckSeen,
  setCommandDeckSeen,
}: CampaignOverlayOptions): CampaignOverlayState {
  const [confirmedStageId, setConfirmedStageId] = useState<string | null>(null)

  const confirmCampaignStage = useCallback(() => {
    setConfirmedStageId(activeStage.id)
  }, [activeStage.id])

  const showCommandDeck = useCallback(() => {
    setConfirmedStageId(null)
    setCommandDeckSeen(true)
    setShowPlayerInfo(true)
  }, [setCommandDeckSeen, setShowPlayerInfo])

  const markCommandDeckSeen = useCallback(() => {
    setCommandDeckSeen(true)
  }, [setCommandDeckSeen])

  const hasSeenCommandDeck = commandDeckSeen || showPlayerInfo
  const campaignUnlocked = hasSeenCommandDeck && readyForGame
  const campaignConfirmed = confirmedStageId === activeStage.id
  const shouldShowCampaignOverlay = campaignUnlocked && !showPlayerInfo && !campaignConfirmed

  const resetCampaignOverlay = useCallback(() => {
    setConfirmedStageId(null)
    setCommandDeckSeen(false)
  }, [setCommandDeckSeen])

  const setCampaignConfirmed = useCallback(
    (value: boolean) => {
      if (value) {
        setConfirmedStageId(activeStage.id)
      } else {
        setConfirmedStageId(null)
      }
    },
    [activeStage.id],
  )

  return {
    campaignUnlocked,
    campaignConfirmed,
    shouldShowCampaignOverlay,
    confirmCampaignStage,
    showCommandDeck,
    markCommandDeckSeen,
    setCampaignConfirmed,
    resetCampaignOverlay,
  }
}

export default useCampaignOverlay
