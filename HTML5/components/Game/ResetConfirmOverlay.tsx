"use client"

import { Button } from "@/components/ui/button"

type ResetConfirmOverlayProps = {
  onCancel: () => void
  onConfirm: () => void
}

export default function ResetConfirmOverlay({ onCancel, onConfirm }: ResetConfirmOverlayProps) {
  return (
    <div className="absolute inset-0 z-50 rounded-[22px] bg-black/70 backdrop-blur flex items-center justify-center">
      <div className="w-full max-w-sm rounded-3xl border border-white/10 bg-gradient-to-b from-slate-900/95 to-rose-950/90 p-6 text-center text-white shadow-[0_30px_120px_rgba(76,29,149,0.6)]">
        <p className="text-xs uppercase tracking-[0.4em] text-white/50 mb-1">Safety Check</p>
        <h2 className="text-2xl font-bold mb-2">Reset Player?</h2>
        <p className="text-sm text-white/70 mb-5">This wipes the current guest session and spawns a new recruit.</p>
        <div className="flex items-center justify-center gap-3">
          <Button onClick={onCancel} size="sm" className="rounded-full bg-white/10 text-white hover:bg-white/20 text-xs">
            Keep Progress
          </Button>
          <Button
            onClick={onConfirm}
            size="sm"
            className="rounded-full bg-rose-500 text-white hover:bg-rose-400 text-xs shadow-lg shadow-rose-900/50"
          >
            Confirm Reset
          </Button>
        </div>
      </div>
    </div>
  )
}
