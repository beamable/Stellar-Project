"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import type { ListingContent, StoreContent } from "beamable-sdk"
import type { BallType, BallTypeConfig } from "@/components/Game/types"
import { waitForMintingDelayOffset } from "@/lib/mintingDelay"

type ShopOverlayProps = {
  store: StoreContent | null
  listings: ListingContent[]
  loading: boolean
  error: string | null
  currencyAmount: number | null
  ballTypeMap: Record<BallType, BallTypeConfig>
  ownedBallTypes: BallType[]
  onClose: () => void
  onRefresh?: () => void
  onPurchase: (listingId: string) => Promise<void>
}

const formatCoins = (value: number | null) =>
  typeof value === "number" ? value.toLocaleString() : "—"

const getPrice = (listing: ListingContent) =>
  (listing as any)?.properties?.price?.data?.amount as number | undefined

const getSymbol = (listing: ListingContent) =>
  (listing as any)?.properties?.price?.data?.symbol as string | undefined

const deriveBallType = (id?: string | null): BallType | null => {
  const candidate = (id ?? "").toLowerCase()
  const slug = candidate.split(".").pop() ?? candidate
  const map: Record<string, BallType> = {
    fireball: "fire",
    fire: "fire",
    multishot: "multishot",
    laserball: "laser",
    laser: "laser",
    normal: "normal",
  }
  return (map[slug] as BallType) ?? null
}

const prettifyName = (id?: string | null, fallback?: string | null) => {
  if (fallback) return fallback
  const raw = id?.split(".").pop() ?? id ?? ""
  if (!raw) return "Listing"
  const spaced = raw.replace(/[_-]+/g, " ")
  const words = spaced.replace(/([a-z])([A-Z])/g, "$1 $2").split(" ")
  return words
    .filter(Boolean)
    .map((w) => w.charAt(0).toUpperCase() + w.slice(1))
    .join(" ")
}

export default function ShopOverlay({
  store,
  listings,
  loading,
  error,
  currencyAmount,
  ballTypeMap,
  ownedBallTypes,
  onClose,
  onRefresh,
  onPurchase,
}: ShopOverlayProps) {
  const [purchasingId, setPurchasingId] = useState<string | null>(null)
  const [refreshDelayActive, setRefreshDelayActive] = useState(false)
  const title = store?.properties?.title?.data ?? "Command Deck Shop"
  const effectiveLoading = loading || refreshDelayActive
  return (
    <div className="fixed inset-0 z-[60] flex items-center justify-center bg-black/80 backdrop-blur px-4 py-8">
      <div className="w-full max-w-6xl rounded-[32px] border border-white/10 bg-gradient-to-br from-slate-950 via-indigo-950/95 to-slate-900/95 p-8 text-white shadow-[0_40px_160px_rgba(2,6,23,0.85)]">
        <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
          <div>
            <p className="text-xs uppercase tracking-[0.4em] text-white/50">Command Deck</p>
            <h2 className="text-3xl font-black tracking-wide">{title}</h2>
            <p className="text-sm text-white/70">Browse listings, synced to live content.</p>
          </div>
          <div className="flex flex-wrap gap-2 justify-end">
            <div className="rounded-full border border-amber-300/30 bg-amber-400/10 px-4 py-2 text-xs font-semibold text-white shadow-inner shadow-amber-900/40">
              <div className="flex items-center gap-2">
                <span className="inline-flex h-6 w-6 items-center justify-center rounded-full bg-gradient-to-br from-amber-200 to-amber-500 text-slate-900 text-xs font-black shadow-inner shadow-amber-900/50">
                  C
                </span>
                <span className="text-white text-sm font-semibold">{formatCoins(currencyAmount)}</span>
                <span className="text-amber-100/70 text-[10px] uppercase tracking-wide">Coins (live)</span>
              </div>
            </div>
            {onRefresh && (
              <Button
                size="sm"
                className="rounded-full bg-white/10 text-white hover:bg-white/20 border border-white/10 text-xs"
                disabled={refreshDelayActive}
                onClick={async () => {
                  if (refreshDelayActive) return
                  setRefreshDelayActive(true)
                  try {
                    onRefresh()
                    await waitForMintingDelayOffset()
                  } finally {
                    setRefreshDelayActive(false)
                  }
                }}
              >
                {refreshDelayActive ? "Refreshing..." : "Refresh"}
              </Button>
            )}
            <Button
              size="sm"
              className="rounded-full bg-emerald-400 text-slate-900 hover:bg-emerald-300 text-xs"
              onClick={onClose}
            >
              Close
            </Button>
          </div>
        </div>

        <div className="mt-6">
          {effectiveLoading ? (
            <div className="rounded-2xl border border-white/10 bg-white/5 p-6 text-white/70 text-sm">
              Syncing store listings from server...
            </div>
          ) : error ? (
            <div className="rounded-2xl border border-rose-400/40 bg-rose-500/10 p-6 text-rose-50 text-sm">
              {error}
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {listings.map((listing) => {
                const price = getPrice(listing) ?? NaN
                const symbol = getSymbol(listing) ?? "currency.coin.beam_coin"
                const obtainItems = ((listing as any)?.properties?.offer?.data?.obtainItems ?? []) as any[]
                const firstItemId = obtainItems[0]?.contentId as string | undefined
                const ballType = deriveBallType(firstItemId)
                const ballConfig = ballType ? ballTypeMap[ballType] : undefined
                const friendlyName = prettifyName(firstItemId, ballConfig?.name)
                const description = ballConfig?.description || "Unlock a new tactical ball for your arsenal."
                const alreadyOwned = ballType ? ownedBallTypes.includes(ballType) : false
                const canBuy =
                  !alreadyOwned &&
                  typeof currencyAmount === "number" &&
                  Number.isFinite(price) &&
                  currencyAmount >= price
                const shortSymbol = symbol.includes(".") ? symbol.split(".").pop() : symbol
                const deficit =
                  typeof currencyAmount === "number" && Number.isFinite(price)
                    ? Math.max(0, price - currencyAmount)
                    : null
                const isPurchasing = purchasingId === listing.id
                return (
                  <div
                    key={listing.id}
                    className="relative rounded-[20px] border border-white/10 bg-gradient-to-br from-slate-900/80 via-indigo-950/80 to-slate-900/70 p-4 shadow-[0_20px_60px_rgba(2,6,23,0.6)]"
                  >
                    <div className="flex items-center justify-between mb-2">
                      <span className="rounded-full bg-white/10 px-3 py-1 text-[11px] uppercase tracking-wide text-white/70">
                        Listing
                      </span>
                      {ballConfig?.icon && (
                        <span className="absolute right-4 top-4 h-10 w-10 rounded-full bg-white/10 border border-white/15 flex items-center justify-center text-lg font-bold text-white/90 shadow-inner shadow-black/40">
                          {ballConfig.icon}
                        </span>
                      )}
                    </div>
                    <h3 className="text-xl font-semibold">{friendlyName}</h3>
                    <p className="text-sm text-white/70 line-clamp-2">{description}</p>

                    <div className="mt-6 rounded-2xl border border-white/10 bg-white/5 px-4 py-4 flex items-center justify-center">
                      <div className="text-center">
                        <div className="text-xs uppercase tracking-wide text-white/60">Price</div>
                        <div className="text-2xl font-black text-amber-200">
                          {Number.isFinite(price) ? price.toLocaleString() : "N/A"}{" "}
                          <span className="text-amber-100/70 text-sm uppercase">{shortSymbol}</span>
                        </div>
                      </div>
                    </div>

                    <Button
                      disabled={!canBuy || isPurchasing}
                      onClick={async () => {
                        if (!listing.id || isPurchasing || alreadyOwned) return
                        setPurchasingId(listing.id)
                        try {
                          await onPurchase(listing.id)
                        } finally {
                          setPurchasingId(null)
                        }
                      }}
                      className={`mt-4 w-full rounded-full text-sm font-semibold shadow-lg ${
                        alreadyOwned
                          ? "bg-rose-500/90 text-white border border-rose-300/60 cursor-not-allowed"
                          : canBuy && !isPurchasing
                            ? "bg-emerald-400 text-slate-900 hover:bg-emerald-300"
                            : "bg-white/5 text-white/60 border border-white/10 cursor-not-allowed"
                      }`}
                    >
                      {alreadyOwned
                        ? "Already Purchased"
                        : isPurchasing
                          ? "Purchasing..."
                          : canBuy
                            ? "Buy"
                            : deficit
                              ? `Need ${deficit.toLocaleString()} more`
                              : "Unavailable"}
                    </Button>
                  </div>
                )
              })}
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
