import type { BallType, BallTypeConfig } from "@/components/Game/types"

type BallSelectionOverlayProps = {
  ballTypes: BallTypeConfig[]
  selectedBallType: BallType
  selectedBallInfo?: BallTypeConfig
  onSelectBall: (ballType: BallType) => void
  onStart: () => void
}

export default function BallSelectionOverlay({
  ballTypes,
  selectedBallType,
  selectedBallInfo,
  onSelectBall,
  onStart,
}: BallSelectionOverlayProps) {
  return (
    <div
      className="absolute inset-0 rounded-[22px] bg-black/70 backdrop-blur-sm flex items-center justify-center z-40"
      onClick={(e) => {
        if (e.target === e.currentTarget) {
          onStart()
        }
      }}
    >
      <div
        className="w-full max-w-2xl rounded-[32px] border border-white/10 bg-gradient-to-b from-slate-900/95 to-indigo-900/90 text-white shadow-[0_30px_120px_rgba(2,6,23,0.8)] p-8"
        onClick={(e) => {
          e.stopPropagation()
        }}
      >
        <p className="text-xs uppercase tracking-[0.4em] text-white/50 mb-2">Loadout</p>
        <h2 className="text-3xl font-black tracking-wide text-white mb-6">Choose Your Ball Type</h2>

        <div className="grid grid-cols-2 md:grid-cols-4 gap-3 mb-6">
          {ballTypes.map((ballType) => (
            <button
              key={ballType.type}
              onClick={() => onSelectBall(ballType.type)}
              className={`rounded-2xl border transition-all px-3 py-4 text-center shadow-inner ${
                selectedBallType === ballType.type
                  ? "border-emerald-300/80 bg-emerald-400/10 shadow-[0_15px_35px_rgba(16,185,129,0.35)]"
                  : "border-white/10 bg-white/5 hover:bg-white/10"
              }`}
            >
              <div className="text-3xl mb-1">{ballType.icon}</div>
              <div className="text-sm font-semibold tracking-wide uppercase text-white/80">{ballType.name}</div>
            </button>
          ))}
        </div>

        {selectedBallInfo && (
          <div className="mb-6 p-4 rounded-2xl bg-white/5 border border-white/10 text-left">
            <div className="flex items-center justify-center gap-2 mb-2">
              <span className="text-xl">{selectedBallInfo.icon}</span>
              <span className="font-semibold text-white">{selectedBallInfo.name}</span>
            </div>
            <p className="text-sm text-white/70">{selectedBallInfo.description}</p>
          </div>
        )}

        <div className="space-y-2 text-sm mb-6 text-white/70">
          <p>Click and hold to aim, release to shoot.</p>
          <p>Destroy every tower to earn your Beam bounty.</p>
          <p className="text-amber-200">Tip: Special blocks (darker) need 2 hits for the bonus.</p>
        </div>

        <p className="text-white font-semibold tracking-wide uppercase text-center text-xs">
          Click outside to deploy your first shot
        </p>
      </div>
    </div>
  )
}
