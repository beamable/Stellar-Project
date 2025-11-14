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
      className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center"
      onClick={(e) => {
        if (e.target === e.currentTarget) {
          onStart()
        }
      }}
    >
      <div
        className="bg-card p-6 rounded-lg border-2 border-primary/30 text-center max-w-lg"
        onClick={(e) => {
          e.stopPropagation()
        }}
      >
        <h2 className="text-2xl font-bold text-primary mb-4">Choose Your Ball Type</h2>

        <div className="grid grid-cols-2 gap-3 mb-6">
          {ballTypes.map((ballType) => (
            <button
              key={ballType.type}
              onClick={() => onSelectBall(ballType.type)}
              className={`p-3 rounded-lg border-2 transition-all hover:scale-105 ${
                selectedBallType === ballType.type
                  ? "border-primary bg-primary/10 shadow-lg"
                  : "border-muted hover:border-primary/50"
              }`}
            >
              <div className="text-2xl mb-1">{ballType.icon}</div>
              <div className="text-sm font-semibold">{ballType.name}</div>
            </button>
          ))}
        </div>

        {selectedBallInfo && (
          <div className="mb-6 p-3 bg-muted/50 rounded-lg">
            <div className="flex items-center justify-center gap-2 mb-2">
              <span className="text-xl">{selectedBallInfo.icon}</span>
              <span className="font-semibold text-primary">{selectedBallInfo.name}</span>
            </div>
            <p className="text-sm text-muted-foreground">{selectedBallInfo.description}</p>
          </div>
        )}

        <div className="space-y-2 text-sm mb-4">
          <p className="text-muted-foreground">Click and hold to aim, release to shoot!</p>
          <p className="text-muted-foreground">Destroy all towers to win.</p>
          <p className="text-accent">Tip: Special blocks (darker) need 2 hits and give double points!</p>
        </div>

        <p className="text-primary font-semibold">Click anywhere outside this window to start playing!</p>
      </div>
    </div>
  )
}
