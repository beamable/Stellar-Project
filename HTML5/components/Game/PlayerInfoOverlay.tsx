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
  onOpenShop: () => void
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
  onOpenShop,
  onClose,
}: PlayerInfoOverlayProps) {
  return (
    <div className="absolute inset-0 rounded-[22px] bg-black/70 backdrop-blur flex items-center justify-center z-40">
      <div className="w-full max-w-3xl rounded-[32px] border border-white/10 bg-gradient-to-br from-slate-900/95 via-indigo-950/90 to-slate-900/95 p-8 text-white shadow-[0_35px_140px_rgba(2,6,23,0.85)]">
        <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4 mb-6">
          <div>
            <p className="text-xs uppercase tracking-[0.4em] text-white/50">Command Deck</p>
            <h2 className="text-3xl font-black">Player Identity</h2>
          </div>
          <div className="flex gap-2 justify-end">
            <Button
              onClick={onResetPlayer}
              size="sm"
              className="rounded-full bg-rose-500 text-white hover:bg-rose-400 text-xs shadow-md shadow-rose-900/50"
            >
              Reset Player
            </Button>
            <Button
              onClick={onOpenShop}
              size="sm"
              className="rounded-full bg-white/10 text-white hover:bg-white/20 border border-white/10 text-xs"
            >
              Open Shop
            </Button>
            <Button
              size="sm"
              onClick={onClose}
              className="rounded-full bg-emerald-400 text-slate-900 hover:bg-emerald-300 text-xs"
            >
              Play Game
            </Button>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-5 text-left">
          <div className="space-y-3">
            <InfoRow label="GamerTag ID" value={playerId} copyable />
            <InfoRow label="Alias" value={alias || "-"} />
            <InfoRow label="Stellar Custodial ID" value={stellarExternalId} copyable />
          </div>
          <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
            <div className="text-xs uppercase tracking-wide text-white/60 mb-2">External Stellar Identity</div>
            <div className="flex items-start gap-3">
              <p className="flex-1 font-mono break-all text-sm">
                {stellarExternalIdentityId || "Not linked yet"}
              </p>
              {stellarExternalIdentityId ? (
                <Button
                  size="sm"
                  className="rounded-full bg-white/10 text-white hover:bg-white/20 text-xs"
                  onClick={async () => {
                    try {
                      await navigator.clipboard.writeText(stellarExternalIdentityId || "")
                    } catch {}
                  }}
                >
                  Copy
                </Button>
              ) : (
                <Button
                  size="sm"
                  className={`rounded-full text-xs ${
                    pendingSignUrl
                      ? "bg-amber-400 text-slate-900 hover:bg-amber-300"
                      : "bg-white/10 text-white hover:bg-white/20"
                  }`}
                  onClick={onAttachClick}
                >
                  {pendingSignUrl ? "Sign Stellar Wallet" : "Attach External ID"}
                </Button>
              )}
            </div>
            <WalletPopupWarning
              blocked={walletPopupBlocked}
              blockedUrl={walletPopupBlockedUrl}
              context={walletPopupContext}
              onManualOpen={onManualWalletOpen}
            />
            {signatureError && (
              <div
                role="alert"
                className="mt-3 rounded-2xl border border-rose-400/40 bg-rose-500/10 px-3 py-2 text-sm text-rose-100 flex items-start gap-3"
              >
                <span className="flex-1">{signatureError}</span>
                <Button
                  size="sm"
                  className="rounded-full bg-white/10 text-white hover:bg-white/20 text-xs"
                  onClick={onRetryAttach}
                >
                  Retry
                </Button>
              </div>
            )}
          </div>
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
    <div className="rounded-2xl border border-white/10 bg-white/5 px-4 py-3 flex items-center justify-between gap-3">
      <div>
        <div className="text-xs uppercase tracking-wide text-white/60">{label}</div>
        <div className="font-mono break-all text-white">{value || "-"}</div>
      </div>
      {copyable && (
        <Button
          size="sm"
          className="rounded-full bg-white/10 text-white hover:bg-white/20 text-xs"
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
