"use client"

import { Button } from "@/components/ui/button"

type ResetConfirmOverlayProps = {
  onCancel: () => void
  onConfirm: () => void
}

export default function ResetConfirmOverlay({ onCancel, onConfirm }: ResetConfirmOverlayProps) {
  return (
    <div className="absolute inset-0 bg-black/60 z-50 rounded-lg flex items-center justify-center">
      <div className="bg-card p-6 rounded-lg border-2 border-primary/30 text-center max-w-md w-full">
        <h2 className="text-2xl font-bold text-primary mb-2">Reset Player?</h2>
        <p className="text-sm text-muted-foreground mb-4">This will create a new guest player for this tab.</p>
        <div className="flex items-center justify-center gap-3">
          <Button onClick={onCancel} variant="outline" size="sm" className="text-xs">
            Cancel
          </Button>
          <Button
            onClick={onConfirm}
            variant="destructive"
            size="sm"
            className="text-xs transition-transform duration-150 hover:scale-105 hover:shadow-lg"
          >
            Yes, Reset
          </Button>
        </div>
      </div>
    </div>
  )
}
