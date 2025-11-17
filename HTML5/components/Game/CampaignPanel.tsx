import { Button } from "@/components/ui/button"
import type { CampaignStage, MechanicTag } from "@/components/Game/campaign"
import { MECHANICS } from "@/components/Game/campaign"
import type { StageProgress } from "@/hooks/useCampaignProgress"

type CampaignPanelProps = {
  activeStage: CampaignStage
  stageProgress: StageProgress[]
  selectedStageId: string
  pendingMechanics: MechanicTag[]
  onSelectStage: (stageId: string) => void
  onAcknowledgeMechanics: () => void
  campaignComplete: boolean
  loopCount: number
  onStartNextLoop: () => void
}

export default function CampaignPanel({
  activeStage,
  stageProgress,
  selectedStageId,
  pendingMechanics,
  onSelectStage,
  onAcknowledgeMechanics,
  campaignComplete,
  loopCount,
  onStartNextLoop,
}: CampaignPanelProps) {
  const stageCount = stageProgress.length
  return (
    <section className="mt-4 rounded-3xl border border-white/10 bg-gradient-to-r from-slate-900/80 via-indigo-950/80 to-slate-900/80 p-6 text-white shadow-inner shadow-black/40">
      <div className="grid gap-6 lg:grid-cols-[3fr_2fr]">
        <div>
          <p className="text-xs uppercase tracking-[0.4em] text-white/50">
            Campaign Stage {activeStage.order + 1} / {stageCount}
          </p>
          <h2 className="text-2xl font-semibold tracking-wide">{activeStage.name}</h2>
          <p className="mt-2 text-sm text-white/70">{activeStage.summary}</p>

          <div className="mt-4 space-y-3">
            <div>
              <p className="text-xs uppercase tracking-wide text-white/50">Objective</p>
              <p className="text-sm text-white">{activeStage.objective}</p>
            </div>
            <div>
              <p className="text-xs uppercase tracking-wide text-white/50">Active Mechanics</p>
              <div className="mt-2 flex flex-wrap gap-2">
                {activeStage.mechanics.length === 0 && (
                  <span className="rounded-full border border-white/10 px-3 py-1 text-xs text-white/60">
                    Baseline run
                  </span>
                )}
                {activeStage.mechanics.map((tag) => {
                  const mechanic = MECHANICS[tag]
                  return (
                    <span
                      key={tag}
                      className="rounded-full border border-white/15 bg-white/10 px-3 py-1 text-xs font-medium text-white"
                    >
                      {mechanic.title}
                    </span>
                  )
                })}
              </div>
            </div>
          </div>
        </div>
        <div>
          <p className="text-xs uppercase tracking-[0.4em] text-white/50">Stage Path</p>
          <div className="mt-3 grid grid-cols-4 gap-2 sm:grid-cols-5 lg:grid-cols-3 xl:grid-cols-4">
            {stageProgress.map((entry) => {
              const stageNumber = entry.stage.order + 1
              const isSelected = entry.stage.id === selectedStageId
              const isLocked = entry.status === "locked"
              const statusColors =
                entry.status === "completed"
                  ? "border-emerald-400/60 bg-emerald-400/15 text-emerald-100"
                  : entry.status === "available"
                    ? "border-amber-300/40 bg-amber-300/10 text-amber-100"
                    : "border-white/10 bg-transparent text-white/30"
              const bossMarker = entry.stage.type === "boss"
              return (
                <button
                  key={entry.stage.id}
                  disabled={isLocked}
                  onClick={() => onSelectStage(entry.stage.id)}
                  className={`flex flex-col items-center rounded-2xl border px-3 py-3 text-xs transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white/40 ${statusColors} ${isSelected ? "ring-2 ring-white/30" : ""} ${isLocked ? "opacity-40 cursor-not-allowed" : "hover:border-white/50 hover:bg-white/10"}`}
                >
                  <span className="text-lg font-semibold leading-none">{stageNumber}</span>
                  <span className="mt-1 text-[0.65rem] uppercase tracking-wide">
                    {bossMarker ? "Boss" : "Stage"}
                  </span>
                  {entry.status === "completed" && <span className="mt-1 text-[0.6rem] text-emerald-200">Clear</span>}
                  {isSelected && entry.status !== "completed" && (
                    <span className="mt-1 text-[0.6rem] text-white/70">Active</span>
                  )}
                </button>
              )
            })}
          </div>
          <div className="mt-4 rounded-2xl border border-white/10 bg-white/5 p-4 text-sm text-white/80">
            <div className="flex items-center justify-between text-xs uppercase tracking-wide text-white/60">
              <span>Loop Status</span>
              <span>Loop {loopCount + 1}</span>
            </div>
            {campaignComplete ? (
              <div className="mt-3 space-y-2">
                <p className="text-sm text-emerald-100">Boss defeated! Start a fresh loop.</p>
                <Button
                  onClick={onStartNextLoop}
                  className="w-full rounded-full bg-emerald-400 text-slate-900 hover:bg-emerald-300"
                >
                  Begin Loop {loopCount + 2}
                </Button>
              </div>
            ) : (
              <p className="mt-3 text-xs text-white/60">Complete all 11 stages to increase your loop counter.</p>
            )}
          </div>
        </div>
      </div>

      {pendingMechanics.length > 0 && (
        <div className="mt-5 rounded-2xl border border-amber-400/40 bg-amber-300/10 p-4">
          <p className="text-xs uppercase tracking-[0.4em] text-amber-200">New Mechanic Unlocked</p>
          <ul className="mt-3 space-y-2 text-sm">
            {pendingMechanics.map((tag) => {
              const mechanic = MECHANICS[tag]
              return (
                <li key={tag}>
                  <span className="font-semibold text-white">{mechanic.title}</span>{" "}
                  <span className="text-white/80">- {mechanic.description}</span>
                </li>
              )
            })}
          </ul>
          <Button
            onClick={onAcknowledgeMechanics}
            size="sm"
            className="mt-4 rounded-full border border-white/10 bg-white/10 text-white hover:bg-white/20"
          >
            Understood
          </Button>
        </div>
      )}
    </section>
  )
}
