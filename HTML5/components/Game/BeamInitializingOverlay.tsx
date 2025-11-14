"use client"

export default function BeamInitializingOverlay() {
  return (
    <div className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center">
      <div className="bg-card p-4 rounded-lg border-2 border-primary/30 text-center">
        <p className="text-lg font-semibold text-primary">Beam is initializing...</p>
      </div>
    </div>
  )
}
