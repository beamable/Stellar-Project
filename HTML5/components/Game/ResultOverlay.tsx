import { Button } from "@/components/ui/button"

export type CampaignResultContext = {
  stageName: string
  stageLabel: string
  stageType: "standard" | "boss"
  nextStageName?: string
  canAdvance: boolean
  campaignComplete: boolean
  loopCount: number
  onAdvance: () => void
  onStartLoop: () => void
}

type ResultOverlayProps = {
  gameState: "playing" | "won" | "gameOver"
  score: number
  coinsEarned: number
  ballsLeft: number
  victoryBonusMultiplier: number
  onRetry: () => void
  campaignContext?: CampaignResultContext
}

export default function ResultOverlay({
  gameState,
  score,
  coinsEarned,
  ballsLeft,
  victoryBonusMultiplier,
  onRetry,
  campaignContext,
}: ResultOverlayProps) {
  if (gameState === "playing") {
    return null
  }

  const isWin = gameState === "won"
  const title = isWin ? "Victory!" : "Game Over"
  const description = isWin ? "All towers destroyed!" : "No balls left!"
  const bonusText =
    isWin && ballsLeft > 0
      ? `Bonus: ${ballsLeft} balls remaining = ${(1 + ballsLeft * victoryBonusMultiplier).toFixed(1)}x multiplier!`
      : null
  const nextLoopLabel = campaignContext ? campaignContext.loopCount + 2 : 2
  const showLoopCta =
    Boolean(
      isWin &&
        campaignContext &&
        campaignContext.campaignComplete &&
        campaignContext.stageType === "boss",
    )
  const showContinueButton = Boolean(isWin && campaignContext?.canAdvance)

  return (
    <div className="absolute inset-0 rounded-[22px] bg-black/80 backdrop-blur flex items-center justify-center z-50">
      <div className="w-full max-w-lg rounded-[32px] border border-white/10 bg-gradient-to-b from-slate-900/95 to-indigo-950/90 p-8 text-center text-white shadow-[0_40px_120px_rgba(0,0,0,0.8)]">
        <p className="text-xs uppercase tracking-[0.4em] text-white/50 mb-2">Mission Report</p>
        <p className={`text-4xl font-black mb-2 ${isWin ? "text-amber-200" : "text-rose-300"}`}>{title}</p>
        <p className="text-white/80 mb-4">{description}</p>

        {campaignContext && (
          <div className="mb-4 rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-left">
            <p className="text-xs uppercase tracking-wide text-white/60">{campaignContext.stageLabel}</p>
            <p className="text-lg font-semibold text-white">{campaignContext.stageName}</p>
            {campaignContext.stageType === "boss" && (
              <p className="text-xs text-rose-200 mt-1">Boss encounter</p>
            )}
          </div>
        )}

        <div className="rounded-2xl border border-white/10 bg-white/5 px-5 py-4 mb-4">
          <p className="text-sm uppercase tracking-wide text-white/60">Final Score</p>
          <p className="text-3xl font-semibold text-white">{score.toLocaleString()}</p>
          {bonusText && <p className="text-xs text-amber-200 mt-2">{bonusText}</p>}
        </div>
        <div className="rounded-2xl border border-amber-200/30 bg-amber-400/10 px-5 py-3 mb-4">
          <p className="text-sm uppercase tracking-wide text-amber-100/80">Coins Earned</p>
          <p className="text-2xl font-semibold text-amber-200">{coinsEarned.toLocaleString()}</p>
        </div>

        <div className="mt-4 flex flex-wrap justify-center gap-3">
          {showContinueButton && campaignContext && (
            <Button
              onClick={campaignContext.onAdvance}
              className="rounded-full bg-emerald-400 text-slate-900 hover:bg-emerald-300 font-semibold px-6"
            >
              {campaignContext.nextStageName ? `Continue to ${campaignContext.nextStageName}` : "Continue"}
            </Button>
          )}
          {showLoopCta && campaignContext && (
            <Button
              onClick={campaignContext.onStartLoop}
              className="rounded-full border border-white/20 bg-white/10 text-white hover:bg-white/20 px-6"
            >
              Begin Loop {nextLoopLabel}
            </Button>
          )}
          <Button
            onClick={onRetry}
            variant="secondary"
            className="rounded-full border border-white/20 bg-white/10 text-white hover:bg-white/20 px-6"
          >
            {isWin ? "Replay Stage" : "Try Again"}
          </Button>
        </div>
      </div>
    </div>
  )
}
