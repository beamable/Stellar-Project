"use client"

import { useEffect, useRef } from "react"
import type { Beam } from "beamable-sdk"
import getBeam from "@/lib/beam"
import { debugLog } from "@/lib/debugLog"

type UseCoinSyncOptions = {
  readyForGame: boolean
  gameState: "playing" | "won" | "gameOver"
  coinsEarned: number
  onInventoryRefresh: () => void
  currencyContentId?: string
}

export function useCoinSync({
  readyForGame,
  gameState,
  coinsEarned,
  onInventoryRefresh,
  currencyContentId = "currency.coin.beam_coin",
}: UseCoinSyncOptions) {
  const coinsSyncedRef = useRef(false)

  useEffect(() => {
    if (gameState === "playing") {
      coinsSyncedRef.current = false
      return
    }
    if (!readyForGame) return
    if (coinsSyncedRef.current) return
    if (coinsEarned <= 0) return
    coinsSyncedRef.current = true
    ;(async () => {
      try {
        const beam = (await getBeam()) as Beam & {
          stellarFederationClient?: { updateCurrency?: (payload: { currencyContentId: string; amount: number }) => Promise<unknown> }
        }
        const client = beam.stellarFederationClient
        if (client?.updateCurrency) {
          const payload = { currencyContentId, amount: coinsEarned }
          debugLog("[Coins] Syncing earned coins to server:", payload)
          await client.updateCurrency(payload)
          onInventoryRefresh()
        } else {
          console.warn("[Coins] StellarFederationClient.updateCurrency unavailable; skipping sync.")
        }
      } catch (err) {
        console.warn("[Coins] Failed to sync earned coins:", err)
      }
    })()
  }, [gameState, readyForGame, coinsEarned, onInventoryRefresh, currencyContentId])
}

export default useCoinSync
