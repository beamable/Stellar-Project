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
    <div className="relative mx-auto">
      <canvas
        ref={canvasRef}
        width={canvasWidth}
        height={canvasHeight}
        className={`${!readyForGame ? "pointer-events-none" : ""} rounded-[22px] border-2 border-white/20 cursor-crosshair bg-transparent shadow-[0_20px_60px_rgba(0,0,0,0.65)]`}
        onPointerDown={handlePointerDown}
        onPointerMove={handlePointerMove}
        onPointerUp={handlePointerUp}
      />
      <GameOverlayManager {...overlayProps} />
    </div>
  )
}
