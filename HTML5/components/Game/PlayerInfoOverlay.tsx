"use client"

import { Button } from "@/components/ui/button"
import WalletPopupWarning from "@/components/Game/WalletPopupWarning"

type PlayerInfoOverlayProps = {
  playerId: string | null
  alias: string | null
  stellarExternalId: string | null
  stellarExternalIdentityId: string | null
  pendingSignUrl: string | null
  signatureError: string | null
  walletPopupBlocked: boolean
  walletPopupBlockedUrl: string | null
  walletPopupContext: string | null
  onAttachClick: () => void
  onRetryAttach: () => void
  onResetPlayer: () => void
  onManualWalletOpen: () => void
  onClose: () => void
}

export default function PlayerInfoOverlay({
  playerId,
  alias,
  stellarExternalId,
  stellarExternalIdentityId,
  pendingSignUrl,
  signatureError,
  walletPopupBlocked,
  walletPopupBlockedUrl,
  walletPopupContext,
  onAttachClick,
  onRetryAttach,
  onResetPlayer,
  onManualWalletOpen,
  onClose,
}: PlayerInfoOverlayProps) {
  return (
    <div className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center">
      <div className="bg-card p-6 rounded-lg border-2 border-primary/30 text-center max-w-lg w-full">
        <h2 className="text-2xl font-bold text-primary mb-4">Player Info</h2>
        <div className="space-y-3 text-left">
          <InfoRow label="GamerTag ID" value={playerId} copyable />
          <InfoRow label="Alias" value={alias || "-"} />
          <InfoRow label="Stellar Custodial ID" value={stellarExternalId} copyable />

          <div className="flex items-start justify-between gap-2">
            <div>
              <div className="text-sm text-muted-foreground">Stellar External ID</div>
              <div className="font-mono break-all">{stellarExternalIdentityId || "-"}</div>
            </div>
            {stellarExternalIdentityId ? (
              <Button
                size="sm"
                variant="outline"
                onClick={async () => {
                  try {
                    await navigator.clipboard.writeText(stellarExternalIdentityId || "")
                  } catch {}
                }}
              >
                Copy
              </Button>
            ) : (
              <div className="flex flex-col gap-2">
                <Button
                  size="sm"
                  variant={pendingSignUrl ? "default" : "outline"}
                  className={
                    pendingSignUrl ? "bg-orange-500 text-white hover:bg-orange-600 border-orange-500" : undefined
                  }
                  onClick={onAttachClick}
                >
                  {pendingSignUrl ? "Sign Stellar Wallet" : "Attach External Id"}
                </Button>
                <WalletPopupWarning
                  blocked={walletPopupBlocked}
                  blockedUrl={walletPopupBlockedUrl}
                  context={walletPopupContext}
                  onManualOpen={onManualWalletOpen}
                />
                {signatureError && (
                  <div
                    role="alert"
                    className="text-sm text-red-700 bg-red-50 border border-red-200 rounded-md px-3 py-2 flex items-start gap-3"
                  >
                    <span className="flex-1">{signatureError}</span>
                    <Button
                      size="sm"
                      variant="outline"
                      className="text-red-700 border-red-300 hover:bg-red-100 px-2 py-1 text-xs"
                      onClick={onRetryAttach}
                    >
                      Retry
                    </Button>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
        <div className="flex items-center justify-center gap-3 mt-5">
          <Button onClick={onResetPlayer} variant="destructive" size="sm">
            Reset Player
          </Button>
          <Button className="bg-primary hover:bg-primary/90" size="sm" onClick={onClose}>
            Play Game
          </Button>
        </div>
      </div>
    </div>
  )
}

type InfoRowProps = {
  label: string
  value: string | null
  copyable?: boolean
}

function InfoRow({ label, value, copyable }: InfoRowProps) {
  return (
    <div className="flex items-center justify-between gap-2">
      <div>
        <div className="text-sm text-muted-foreground">{label}</div>
        <div className="font-mono break-all">{value || "-"}</div>
      </div>
      {copyable && (
        <Button
          size="sm"
          variant="outline"
          onClick={async () => {
            try {
              await navigator.clipboard.writeText(value || "")
            } catch {}
          }}
        >
          Copy
        </Button>
      )}
    </div>
  )
}
