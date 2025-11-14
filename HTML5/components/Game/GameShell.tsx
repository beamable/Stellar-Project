"use client"

import type React from "react"
import { Card } from "@/components/ui/card"
import GameHud from "@/components/Game/GameHud"
import type { GameSurfaceProps } from "@/components/Game/GameSurface"
import GameSurface from "@/components/Game/GameSurface"
import type { BallTypeConfig } from "@/components/Game/types"

type GameShellProps = {
  hudProps: {
    score: number
    ballsLeft: number
    remainingTowers: number
    towerCount: number
    alias: string | null
    playerId: string | null
    isCharging: boolean
    powerSnapshot: number
    selectedBallInfo?: BallTypeConfig
    onResetPlayer: () => void
    canShowRestart: boolean
    onRestart: () => void
  }
  surfaceProps: GameSurfaceProps
}

export default function GameShell({ hudProps, surfaceProps }: GameShellProps) {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-4 bg-black">
      <Card className="p-6 bg-card border-2 border-primary/20 shadow-2xl">
        <GameHud {...hudProps} />
        <GameSurface {...surfaceProps} />
      </Card>
    </div>
  )
}
