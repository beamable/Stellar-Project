"use client"

import { useCallback, useState } from "react"
import { debugLog } from "@/lib/debugLog"
import getBeam from "@/lib/beam"
import { fetchInventory } from "@/lib/beamInventory"
import type { Beam } from "beamable-sdk"

type UseShopOptions = {
  inventoryInitialized: boolean
  onHidePlayerInfo?: () => void
  onRefreshInventory: () => void
}

export type ShopState = {
  showShop: boolean
  openShop: () => void
  closeShop: () => void
  purchaseListing: (listingId: string) => Promise<void>
}

export function useShop({ inventoryInitialized, onHidePlayerInfo, onRefreshInventory }: UseShopOptions): ShopState {
  const [showShop, setShowShop] = useState(false)

  const openShop = useCallback(() => {
    if (!inventoryInitialized) return
    onHidePlayerInfo?.()
    setShowShop(true)
  }, [inventoryInitialized, onHidePlayerInfo])

  const closeShop = useCallback(() => {
    setShowShop(false)
  }, [])

  const purchaseListing = useCallback(
    async (listingId: string) => {
      try {
        const beam = (await getBeam()) as Beam & {
          stellarFederationClient?: { purchaseBall?: (payload: { purchaseId: string }) => Promise<unknown> }
        }
        if (beam.stellarFederationClient?.purchaseBall) {
          debugLog("[Commerce] Purchasing listing:", listingId)
          await beam.stellarFederationClient.purchaseBall({ purchaseId: listingId })
          await fetchInventory(beam)
          onRefreshInventory()
        } else {
          throw new Error("PurchaseBall unavailable on StellarFederationClient")
        }
      } catch (err) {
        console.warn("[Commerce] Purchase failed:", err)
        throw err
      }
    },
    [onRefreshInventory],
  )

  return {
    showShop,
    openShop,
    closeShop,
    purchaseListing,
  }
}

export default useShop
