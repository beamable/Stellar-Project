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
  isAudioSettingsOpen: boolean
  onToggleAudioSettings: () => void
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
  isAudioSettingsOpen,
  onToggleAudioSettings,
}: GameHudProps) {
  const identityLabel = alias ?? playerId ?? "Guest"
  const selectedBallIcon = selectedBallInfo?.icon ?? "dYZ_"
  const powerPercent = Math.min(100, Math.max(0, powerSnapshot))

  const stats = [
    { label: "Score", value: score.toLocaleString(), accent: "text-cyan-100", icon: "dY?+" },
    { label: "Balls", value: ballsLeft.toString(), accent: "text-emerald-100", icon: selectedBallIcon },
    { label: "Towers", value: `${remainingTowers}/${towerCount}`, accent: "text-amber-100", icon: "dY-�" },
    { label: "Identity", value: identityLabel, accent: "text-pink-100", icon: "dY`" },
  ]

  return (
    <section className="mb-6">
      <div className="rounded-3xl border border-white/10 bg-gradient-to-r from-slate-900 via-indigo-900 to-purple-900 text-white shadow-2xl shadow-indigo-900/40 px-6 py-5">
        <div className="flex flex-wrap items-center justify-between gap-4">
          <div>
            <p className="text-xs uppercase tracking-[0.4em] text-white/50">Beamable Presents</p>
            <h1 className="text-3xl font-black tracking-wider drop-shadow-md">Tower Destroyer</h1>
          </div>
          <div className="flex flex-wrap gap-2">
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
              onClick={onResetPlayer}
              size="sm"
              className="bg-rose-500 text-white hover:bg-rose-400 rounded-full text-xs shadow-lg shadow-rose-900/40"
            >
              Reset Player
            </Button>
          </div>
        </div>

        <div className="mt-4 grid grid-cols-2 gap-3 md:grid-cols-4">
          {stats.map((stat) => (
            <div
              key={stat.label}
              className="rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-left backdrop-blur-[2px]"
            >
              <div className="text-xs uppercase tracking-wide text-white/60 flex items-center gap-1">
                <span>{stat.icon}</span>
                {stat.label}
              </div>
              <p className={`text-xl font-semibold ${stat.accent}`}>{stat.value}</p>
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
              className={`h-full rounded-full transition-all duration-300 ${isCharging ? "bg-amber-300" : "bg-emerald-300"}`}
              style={{ width: `${powerPercent}%` }}
            />
          </div>
        </div>
      </div>
    </section>
  )
}
