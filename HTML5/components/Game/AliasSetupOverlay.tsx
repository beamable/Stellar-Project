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
    <div className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center">
      <div className="bg-card p-6 rounded-lg border-2 border-primary/30 text-center max-w-md w-full">
        <h2 className="text-2xl font-bold text-primary mb-4">Set Your Alias</h2>
        <p className="text-sm text-muted-foreground mb-3">Alphabet letters only, minimum 3 characters.</p>
        <input
          type="text"
          value={aliasInput}
          onChange={(e) => onAliasChange(e.target.value)}
          className="w-full p-2 border rounded mb-3 bg-background text-foreground border-primary/30"
          placeholder="Enter alias"
        />
        {aliasError && <p className="text-destructive text-sm mb-2">{aliasError}</p>}
        <div className="flex gap-2 justify-center">
          <Button onClick={onSaveAlias} disabled={aliasSaving || !canSave} className="bg-primary hover:bg-primary/90">
            {aliasSaving ? "Saving..." : "Save Alias"}
          </Button>
        </div>
      </div>
    </div>
  )
}
