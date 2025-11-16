import { Slider } from "@/components/ui/slider"
import { Button } from "@/components/ui/button"

type AudioSettingsOverlayProps = {
  volume: number
  onVolumeChange: (volume: number) => void
  onClose: () => void
}

export default function AudioSettingsOverlay({ volume, onVolumeChange, onClose }: AudioSettingsOverlayProps) {
  const volumePercent = Math.round(volume * 100)

  return (
    <div className="absolute inset-0 z-50 flex items-center justify-center rounded-[22px] bg-black/70 backdrop-blur-sm">
      <div className="w-full max-w-lg rounded-[32px] border border-white/10 bg-gradient-to-b from-slate-900/95 to-indigo-900/90 px-8 py-10 text-white shadow-[0_30px_120px_rgba(2,6,23,0.8)]">
        <div className="flex items-start justify-between gap-4 mb-6">
          <div>
            <p className="text-xs uppercase tracking-[0.4em] text-white/50">Audio</p>
            <h2 className="text-3xl font-black tracking-wide text-white">Sound Settings</h2>
            <p className="text-sm text-white/70 mt-1">
              Adjust the global volume for all sound effects in Tower Destroyer.
            </p>
          </div>
          <Button
            type="button"
            size="sm"
            className="rounded-full border border-white/20 bg-white/10 text-white hover:bg-white/20"
            onClick={onClose}
          >
            Close
          </Button>
        </div>

        <div className="rounded-2xl border border-white/10 bg-white/5 px-6 py-8 space-y-5 text-sm">
          <div className="flex items-center justify-between text-xs uppercase tracking-wide text-white/60">
            <span>Master Volume</span>
            <span className="text-white font-semibold">{volumePercent}%</span>
          </div>
          <Slider
            value={[volumePercent]}
            max={100}
            min={0}
            onValueChange={(values) => {
              const [next] = values
              onVolumeChange(Math.min(100, Math.max(0, next ?? 0)) / 100)
            }}
            aria-label="Master volume"
          />
          <div className="flex items-center justify-between pt-2">
            <p className="text-white/70">
              Volume is applied immediately. Use this slider to balance Tower Destroyer with your music or streams.
            </p>
            <Button
              type="button"
              size="sm"
              className="rounded-full border border-white/10 bg-white/10 text-white hover:bg-white/20"
              onClick={() => onVolumeChange(volumePercent === 0 ? 1 : 0)}
            >
              {volumePercent === 0 ? "Unmute" : "Mute"}
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}
