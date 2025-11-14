"use client"

export default function BeamInitializingOverlay() {
  return (
    <div className="absolute inset-0 rounded-[22px] bg-slate-950/80 backdrop-blur flex items-center justify-center z-50">
      <div className="rounded-3xl border border-white/10 bg-gradient-to-r from-indigo-900/90 to-purple-900/90 px-6 py-5 text-center text-white shadow-[0_20px_80px_rgba(2,6,23,0.85)]">
        <p className="text-xs uppercase tracking-[0.4em] text-white/50">Systems</p>
        <p className="text-2xl font-bold">Beam is initializing…</p>
        <p className="text-sm text-white/60 mt-2">Establishing secure link to your command deck.</p>
      </div>
    </div>
  )
}
