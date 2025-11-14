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
    <div className="min-h-screen w-full bg-gradient-to-br from-slate-950 via-indigo-950 to-slate-900 flex items-center justify-center px-4 py-6">
      <Card className="w-full max-w-[1300px] rounded-[32px] border border-white/10 bg-white/5 p-6 shadow-[0_25px_80px_rgba(15,23,42,0.8)] backdrop-blur-xl">
        <GameHud {...hudProps} />
        <div className="mt-4 flex justify-center">
          <div className="rounded-[26px] border border-white/10 bg-black/40 p-3 shadow-inner shadow-black/60">
            <GameSurface {...surfaceProps} />
          </div>
        </div>
      </Card>
    </div>
  )
}
