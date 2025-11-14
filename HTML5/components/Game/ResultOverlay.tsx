import { Button } from "@/components/ui/button"

type ResultOverlayProps = {
  gameState: "playing" | "won" | "gameOver"
  score: number
  ballsLeft: number
  victoryBonusMultiplier: number
  onPlayAgain: () => void
}

export default function ResultOverlay({
  gameState,
  score,
  ballsLeft,
  victoryBonusMultiplier,
  onPlayAgain,
}: ResultOverlayProps) {
  if (gameState === "playing") {
    return null
  }

  const isWin = gameState === "won"
  const title = isWin ? "🏆 Victory! 🏆" : "Game Over"
  const description = isWin ? "All towers destroyed!" : "No balls left!"
  const bonusText =
    isWin && ballsLeft > 0
      ? `Bonus: ${ballsLeft} balls remaining = ${(1 + ballsLeft * victoryBonusMultiplier).toFixed(1)}x multiplier!`
      : null

  return (
    <div className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center">
      <div className="bg-card p-6 rounded-lg border-2 border-primary/30 text-center">
        <p className={`text-3xl font-bold mb-2 ${isWin ? "text-primary" : "text-destructive"}`}>{title}</p>
        <p className="text-accent mb-2">
          {description} Final Score: {score}
        </p>
        {bonusText && <p className="text-sm text-primary mb-4">{bonusText}</p>}
        <Button onClick={onPlayAgain} className="bg-primary hover:bg-primary/90">
          Play Again
        </Button>
      </div>
    </div>
  )
}
