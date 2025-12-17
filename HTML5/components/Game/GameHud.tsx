import { useEffect, useRef } from "react"
import { Button } from "@/components/ui/button"
import type { BallTypeConfig } from "@/components/Game/types"

export type GameHudProps = {
  score: number
  ballsLeft: number
  remainingTowers: number
  towerCount: number
  stageLabel: string
  stageName: string
  loopLabel: string
  currencyAmount?: number | null
  alias: string | null
  playerId: string | null
  isCharging: boolean
  powerSnapshot: number
  selectedBallInfo?: BallTypeConfig
  onResetPlayer: () => void
  canShowRestart: boolean
  onRestart: () => void
  isAudioSettingsOpen: boolean
  onToggleAudioSettings: () => void
  onShowCommandDeck: () => void
  showDebugControls?: boolean
  onDebugSkipStage?: () => void
  onDebugFakeLogin?: () => void
}

export default function GameHud({
  score,
  ballsLeft,
  remainingTowers,
  towerCount,
  stageLabel,
  stageName,
  loopLabel,
  currencyAmount,
  alias,
  playerId,
  isCharging,
  powerSnapshot,
  selectedBallInfo,
  onResetPlayer,
  canShowRestart,
  onRestart,
  isAudioSettingsOpen,
  onToggleAudioSettings,
  onShowCommandDeck,
  showDebugControls = false,
  onDebugSkipStage,
  onDebugFakeLogin,
}: GameHudProps) {
  const identityLabel = alias ?? playerId ?? "Guest"
  const powerPercent = Math.min(100, Math.max(0, powerSnapshot))
  const powerFillRef = useRef<HTMLDivElement>(null)
  const lowBallsWarning = ballsLeft < 4

  useEffect(() => {
    const el = powerFillRef.current
    if (!el) return
    el.style.width = `${powerPercent}%`
  }, [powerPercent])

  const stats = [
    { label: "Stage", value: stageLabel, detail: stageName, accent: "text-indigo-100", icon: "ST" },
    { label: "Score", value: score.toLocaleString(), accent: "text-cyan-100", icon: "SC" },
    {
      label: "Balls",
      value: ballsLeft.toString(),
      accent: lowBallsWarning ? "text-red-500" : "text-emerald-100",
      valueClass: lowBallsWarning ? "text-[1.375rem]" : "text-xl",
      icon: selectedBallInfo?.icon ?? "BL",
    },
    { label: "Towers", value: `${remainingTowers}/${towerCount}`, accent: "text-amber-100", icon: "TW" },
    { label: "Identity", value: identityLabel, detail: loopLabel, accent: "text-pink-100", icon: "ID" },
  ]

  return (
    <section className="mb-6">
      <div className="rounded-3xl border border-white/10 bg-gradient-to-r from-slate-900 via-indigo-900 to-purple-900 text-white shadow-2xl shadow-indigo-900/40 px-6 py-5">
        <div className="flex flex-wrap items-center justify-between gap-4">
          <div>
            <p className="text-xs uppercase tracking-[0.4em] text-white/50">Beamable Presents</p>
            <h1 className="text-3xl font-black tracking-wider drop-shadow-md">Tower Destroyer</h1>
          </div>
          <div className="flex flex-wrap items-center gap-2">
            <div className="rounded-full border border-amber-200/40 bg-amber-400/10 px-3 py-2 text-xs font-semibold text-white flex items-center gap-2 shadow-inner shadow-amber-900/40">
              <span className="text-amber-200">◎</span>
              <span className="text-white">{typeof currencyAmount === "number" ? currencyAmount.toLocaleString() : "--"}</span>
              <span className="text-amber-100/70 text-[10px] uppercase tracking-wide">Coins</span>
            </div>
            <Button
              onClick={onToggleAudioSettings}
              size="sm"
              className="rounded-full border border-white/10 bg-white/10 text-white text-xs hover:bg-white/20"
            >
              {isAudioSettingsOpen ? "Close Audio" : "Audio Menu"}
            </Button>
            {canShowRestart && (
              <Button
                onClick={onRestart}
                size="sm"
                className="bg-white/10 text-white hover:bg-white/20 border border-white/10 rounded-full text-xs"
              >
                Restart Run
              </Button>
            )}
            <Button
              onClick={onShowCommandDeck}
              size="sm"
              className="bg-white/10 text-white hover:bg-white/20 border border-white/10 rounded-full text-xs"
            >
              Main Screen
            </Button>
            {showDebugControls && (
              <>
                <Button
                  onClick={onDebugSkipStage}
                  size="sm"
                  className="bg-emerald-400 text-slate-900 hover:bg-emerald-300 rounded-full text-xs"
                >
                  Skip Stage
                </Button>
                <Button
                  onClick={onDebugFakeLogin}
                  size="sm"
                  className="bg-white/10 text-white hover:bg-white/20 border border-white/10 rounded-full text-xs"
                >
                  Fake Login
                </Button>
              </>
            )}
            <Button
              onClick={onResetPlayer}
              size="sm"
              className="bg-rose-500 text-white hover:bg-rose-400 rounded-full text-xs shadow-lg shadow-rose-900/40"
            >
              Reset Player
            </Button>
          </div>
        </div>

        <div className="mt-4 grid grid-cols-2 gap-3 md:grid-cols-5">
          {stats.map((stat) => (
            <div
              key={stat.label}
              className="rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-left backdrop-blur-[2px]"
            >
              <div className="text-xs uppercase tracking-wide text-white/60 flex items-center gap-1">
                <span>{stat.icon}</span>
                {stat.label}
              </div>
              <p className={`${stat.valueClass ?? "text-xl"} font-semibold ${stat.accent}`}>{stat.value}</p>
              {stat.detail && <p className="text-xs text-white/60">{stat.detail}</p>}
            </div>
          ))}
        </div>

        <div className="mt-5 space-y-4">
          <div className="flex items-center justify-between text-xs uppercase tracking-wide text-white/60">
            <span>{isCharging ? "Charging Shot" : "Ready to Fire"}</span>
            <span className="text-white">{powerPercent}%</span>
          </div>
          <div className="h-2 rounded-full bg-white/10 overflow-hidden">
            <div
              ref={powerFillRef}
              className={`h-full rounded-full transition-all duration-300 ${isCharging ? "bg-amber-300" : "bg-emerald-300"}`}
            />
          </div>
        </div>
      </div>
    </section>
  )
}
