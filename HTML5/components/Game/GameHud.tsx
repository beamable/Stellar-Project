import { Button } from "@/components/ui/button"
import type { BallTypeConfig } from "@/components/Game/types"

type GameHudProps = {
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

export default function GameHud({
  score,
  ballsLeft,
  remainingTowers,
  towerCount,
  alias,
  playerId,
  isCharging,
  powerSnapshot,
  selectedBallInfo,
  onResetPlayer,
  canShowRestart,
  onRestart,
}: GameHudProps) {
  return (
    <div className="text-center mb-4">
      <h1 className="text-4xl font-bold text-primary mb-2 font-mono">Tower Destroyer</h1>
      <div className="flex items-center justify-between gap-4 text-lg font-semibold">
        <div className="flex justify-center gap-8 grow">
          <span className="text-accent">Score: {score}</span>
          <span className="text-secondary flex items-center gap-1">
            {selectedBallInfo && <span className="text-base">{selectedBallInfo.icon}</span>}
            Balls: {ballsLeft}
          </span>
          <span className="text-muted-foreground">
            Towers: {remainingTowers}/{towerCount}
          </span>
          {alias ? (
            <span className="text-muted-foreground">Alias: {alias}</span>
          ) : (
            playerId && <span className="text-muted-foreground">Player: {playerId}</span>
          )}
          {isCharging && <span className="text-destructive">Power: {powerSnapshot}%</span>}
        </div>
        <Button
          onClick={onResetPlayer}
          variant="destructive"
          size="sm"
          className="text-xs transition-transform duration-150 hover:scale-105 hover:shadow-lg"
        >
          Reset Player
        </Button>
      </div>
      {canShowRestart && (
        <div className="mt-2 flex gap-2">
          <Button onClick={onRestart} variant="outline" size="sm" className="text-xs bg-transparent">
            Restart Game
          </Button>
        </div>
      )}
    </div>
  )
}
