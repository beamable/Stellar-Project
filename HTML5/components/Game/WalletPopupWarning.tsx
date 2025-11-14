"use client"

type WalletPopupWarningProps = {
  blocked: boolean
  blockedUrl: string | null
  context: string | null
  onManualOpen?: (url: string) => void
}

export default function WalletPopupWarning({ blocked, blockedUrl, context, onManualOpen }: WalletPopupWarningProps) {
  if (!blocked) return null
  return (
    <div className="mt-3 rounded-2xl border border-amber-200/30 bg-amber-500/10 px-4 py-3 text-sm text-amber-100">
      <p className="leading-snug">
        Your browser blocked the {context || "Stellar wallet"} popup. Allow popups for this site and try again.
      </p>
      {blockedUrl && (
        <p className="mt-2">
          Or open the wallet manually:&nbsp;
          <a
            href={blockedUrl}
            target="_blank"
            rel="noreferrer"
            className="underline font-semibold text-amber-200"
            onClick={(event) => {
              if (!onManualOpen) return
              event.preventDefault()
              onManualOpen(blockedUrl)
            }}
          >
            Open Stellar Wallet
          </a>
        </p>
      )}
    </div>
  )
}
