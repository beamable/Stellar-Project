"use client"

import type React from "react"
import GameOverlayManager from "@/components/Game/GameOverlayManager"

export type GameSurfaceProps = {
  canvasRef: React.RefObject<HTMLCanvasElement>
  canvasWidth: number
  canvasHeight: number
  readyForGame: boolean
  handlePointerDown: (event: React.PointerEvent<HTMLCanvasElement>) => void
  handlePointerMove: (event: React.PointerEvent<HTMLCanvasElement>) => void
  handlePointerUp: (event: React.PointerEvent<HTMLCanvasElement>) => void
  overlayProps: React.ComponentProps<typeof GameOverlayManager>
}

export default function GameSurface({
  canvasRef,
  canvasWidth,
  canvasHeight,
  readyForGame,
  handlePointerDown,
  handlePointerMove,
  handlePointerUp,
  overlayProps,
}: GameSurfaceProps) {
  return (
    <div className="relative">
      <canvas
        ref={canvasRef}
        width={canvasWidth}
        height={canvasHeight}
        className={`${!readyForGame ? "pointer-events-none" : ""} border-4 border-primary/30 rounded-lg cursor-crosshair bg-gradient-to-b from-blue-200 to-yellow-200`}
        onPointerDown={handlePointerDown}
        onPointerMove={handlePointerMove}
        onPointerUp={handlePointerUp}
      />
      <GameOverlayManager {...overlayProps} />
    </div>
  )
}
