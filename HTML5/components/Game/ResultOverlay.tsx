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
    <div className="absolute inset-0 rounded-[22px] bg-black/80 backdrop-blur flex items-center justify-center z-50">
      <div className="w-full max-w-lg rounded-[32px] border border-white/10 bg-gradient-to-b from-slate-900/95 to-indigo-950/90 p-8 text-center text-white shadow-[0_40px_120px_rgba(0,0,0,0.8)]">
        <p className="text-xs uppercase tracking-[0.4em] text-white/50 mb-2">Mission Report</p>
        <p className={`text-4xl font-black mb-2 ${isWin ? "text-amber-200" : "text-rose-300"}`}>{title}</p>
        <p className="text-white/80 mb-4">{description}</p>

        <div className="rounded-2xl border border-white/10 bg-white/5 px-5 py-4 mb-4">
          <p className="text-sm uppercase tracking-wide text-white/60">Final Score</p>
          <p className="text-3xl font-semibold text-white">{score.toLocaleString()}</p>
          {bonusText && <p className="text-xs text-amber-200 mt-2">{bonusText}</p>}
        </div>

        <Button
          onClick={onPlayAgain}
          className="rounded-full bg-emerald-400 text-slate-900 hover:bg-emerald-300 font-semibold px-6"
        >
          Play Again
        </Button>
      </div>
    </div>
  )
}
