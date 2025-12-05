"use client"

import { useEffect, useRef } from "react"
import type { CampaignStage } from "@/components/Game/campaign"

type UseWinLoopHandlerOptions = {
  gameState: "playing" | "won" | "gameOver"
  activeStage: CampaignStage
  totalStages: number
  campaignComplete: boolean
  markStageComplete: (stageId: string) => void
  startNextLoop: () => void
  setCampaignConfirmed: (value: boolean) => void
  onLoopAdvance: () => void
}

export function useWinLoopHandler({
  gameState,
  activeStage,
  totalStages,
  campaignComplete,
  markStageComplete,
  startNextLoop,
  setCampaignConfirmed,
  onLoopAdvance,
}: UseWinLoopHandlerOptions) {
  const campaignWinStageRef = useRef<string | null>(null)
  const loopAdvanceRef = useRef(false)

  useEffect(() => {
    if (gameState !== "won") {
      if (gameState === "playing") {
        campaignWinStageRef.current = null
        loopAdvanceRef.current = false
      }
      return
    }
    if (campaignWinStageRef.current && campaignWinStageRef.current !== activeStage.id) {
      return
    }
    if (!campaignWinStageRef.current) {
      campaignWinStageRef.current = activeStage.id
      markStageComplete(activeStage.id)
    }
    if (activeStage.order === totalStages - 1 && campaignComplete && !loopAdvanceRef.current) {
      loopAdvanceRef.current = true
      startNextLoop()
      setCampaignConfirmed(false)
      onLoopAdvance()
    }
  }, [
    gameState,
    activeStage.id,
    activeStage.order,
    totalStages,
    campaignComplete,
    markStageComplete,
    startNextLoop,
    setCampaignConfirmed,
    onLoopAdvance,
  ])
}

export default useWinLoopHandler
