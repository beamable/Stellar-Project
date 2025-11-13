"use client"

type WalletPopupWarningProps = {
  blocked: boolean
  blockedUrl: string | null
  context: string | null
}

export default function WalletPopupWarning({ blocked, blockedUrl, context }: WalletPopupWarningProps) {
  if (!blocked) return null
  return (
    <div className="text-sm text-amber-800 bg-amber-50 border border-amber-200 rounded-md px-3 py-2 mt-3">
      <p>
        Your browser blocked the {context || "Stellar wallet"} popup. Please allow popups for this site and click the
        button again.
      </p>
      {blockedUrl && (
        <p className="mt-2">
          Or open the wallet manually:&nbsp;
          <a href={blockedUrl} target="_blank" rel="noreferrer" className="underline font-semibold">
            Open Stellar Wallet
          </a>
        </p>
      )}
    </div>
  )
}
