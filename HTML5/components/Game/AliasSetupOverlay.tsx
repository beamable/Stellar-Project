"use client"

import { Button } from "@/components/ui/button"

type AliasSetupOverlayProps = {
  aliasInput: string
  aliasError: string | null
  aliasSaving: boolean
  canSave: boolean
  onAliasChange: (value: string) => void
  onSaveAlias: () => void
}

export default function AliasSetupOverlay({
  aliasInput,
  aliasError,
  aliasSaving,
  canSave,
  onAliasChange,
  onSaveAlias,
}: AliasSetupOverlayProps) {
  return (
    <div className="absolute inset-0 rounded-[22px] bg-black/70 backdrop-blur flex items-center justify-center z-40">
      <div className="w-full max-w-md rounded-[32px] border border-white/10 bg-gradient-to-b from-slate-900/95 to-indigo-950/90 p-7 text-center text-white shadow-[0_30px_120px_rgba(2,6,23,0.8)]">
        <p className="text-xs uppercase tracking-[0.4em] text-white/50 mb-2">Identity</p>
        <h2 className="text-3xl font-black mb-2">Set Your Alias</h2>
        <p className="text-sm text-white/70 mb-5">Letters only, at least 3 characters.</p>
        <input
          type="text"
          value={aliasInput}
          onChange={(e) => onAliasChange(e.target.value)}
          className="w-full rounded-2xl border border-white/15 bg-black/40 px-4 py-3 text-center text-white uppercase tracking-widest focus:outline-none focus:ring-2 focus:ring-emerald-300/60"
          placeholder="Enter alias"
        />
        {aliasError && <p className="text-rose-300 text-sm mt-2">{aliasError}</p>}
        <div className="flex gap-2 justify-center mt-5">
          <Button
            onClick={onSaveAlias}
            disabled={aliasSaving || !canSave}
            className="rounded-full bg-emerald-400 text-slate-900 hover:bg-emerald-300 disabled:opacity-40"
          >
            {aliasSaving ? "Saving…" : "Save Alias"}
          </Button>
        </div>
      </div>
    </div>
  )
}
