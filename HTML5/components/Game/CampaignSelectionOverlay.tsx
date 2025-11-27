import { Button } from "@/components/ui/button"
import CampaignPanel from "@/components/Game/CampaignPanel"
import type { CampaignStage, MechanicTag } from "@/components/Game/campaign"
import type { StageProgress } from "@/hooks/useCampaignProgress"

type CampaignSelectionOverlayProps = {
  activeStage: CampaignStage
  stageProgress: StageProgress[]
  selectedStageId: string
  pendingMechanics: MechanicTag[]
  campaignComplete: boolean
  loopCount: number
  onStartNextLoop: () => void
  onSelectStage: (stageId: string) => void
  onAcknowledgeMechanics: () => void
  onConfirm: () => void
  onOpenShop: () => void
}

export default function CampaignSelectionOverlay({
  activeStage,
  stageProgress,
  selectedStageId,
  pendingMechanics,
  campaignComplete,
  loopCount,
  onStartNextLoop,
  onSelectStage,
  onAcknowledgeMechanics,
  onConfirm,
  onOpenShop,
}: CampaignSelectionOverlayProps) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/80 backdrop-blur px-4 py-8">
      <div className="w-full max-w-5xl rounded-[32px] border border-white/10 bg-gradient-to-b from-slate-950/95 to-indigo-950/90 p-8 text-white shadow-[0_40px_160px_rgba(2,6,23,0.85)]">
        <p className="text-xs uppercase tracking-[0.4em] text-white/50 mb-2">Command Deck</p>
        <h2 className="text-3xl font-black tracking-wide">Select Your Next Stage</h2>
        <p className="text-sm text-white/70">
          Choose a target and confirm to brief your pilots. You can always return here after each run.
        </p>

        <CampaignPanel
          activeStage={activeStage}
          stageProgress={stageProgress}
          selectedStageId={selectedStageId}
          pendingMechanics={pendingMechanics}
          onSelectStage={onSelectStage}
          onAcknowledgeMechanics={onAcknowledgeMechanics}
          campaignComplete={campaignComplete}
          loopCount={loopCount}
          onStartNextLoop={onStartNextLoop}
        />

        <div className="mt-6 flex justify-end gap-3">
          <Button
            onClick={onOpenShop}
            variant="outline"
            className="rounded-full border-white/20 text-white hover:bg-white/10"
          >
            Open Shop
          </Button>
          <Button
            onClick={onConfirm}
            className="rounded-full bg-emerald-400 text-slate-900 hover:bg-emerald-300 px-6 font-semibold"
          >
            Lock Stage & Continue
          </Button>
        </div>
      </div>
    </div>
  )
}
