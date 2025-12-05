"use client"

import { useCallback, useEffect, useState } from "react"
import type { CampaignStage } from "@/components/Game/campaign"

type CampaignOverlayOptions = {
  readyForGame: boolean
  activeStage: CampaignStage
  showPlayerInfo: boolean
  setShowPlayerInfo: (value: boolean) => void
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
}: CampaignOverlayOptions): CampaignOverlayState {
  const [confirmedStageId, setConfirmedStageId] = useState<string | null>(null)
  const [commandDeckSeenState, setCommandDeckSeenState] = useState(false)
  useEffect(() => {
    if (!showPlayerInfo) return
    if (commandDeckSeenState) return
    setCommandDeckSeenState(true)
  }, [commandDeckSeenState, showPlayerInfo])

  const confirmCampaignStage = useCallback(() => {
    setConfirmedStageId(activeStage.id)
  }, [activeStage.id])

  const showCommandDeck = useCallback(() => {
    setConfirmedStageId(null)
    setShowPlayerInfo(true)
  }, [setShowPlayerInfo])

  const markCommandDeckSeen = useCallback(() => {
    setCommandDeckSeenState(true)
  }, [])

  const commandDeckSeen = commandDeckSeenState || showPlayerInfo
  const campaignUnlocked = commandDeckSeen && readyForGame
  const campaignConfirmed = confirmedStageId === activeStage.id
  const shouldShowCampaignOverlay = campaignUnlocked && !showPlayerInfo && !campaignConfirmed

  const resetCampaignOverlay = useCallback(() => {
    setConfirmedStageId(null)
    setCommandDeckSeenState(false)
  }, [])

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
