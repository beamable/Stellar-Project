"use client"

import { useEffect, useState } from "react"
import getBeam from "@/lib/beam"
import { fetchInventory } from "@/lib/beamInventory"

type UseCurrencyResult = {
  amount: number | null
  loading: boolean
}

const DEFAULT_CURRENCY_ID = "currency.coins"

export default function useCurrency(
  readyForGame: boolean,
  refreshKey = 0,
  currencyId: string = DEFAULT_CURRENCY_ID,
): UseCurrencyResult {
  const [amount, setAmount] = useState<number | null>(null)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (!readyForGame) {
      setAmount(null)
      return
    }

    let cancelled = false
    let unsubscribe: (() => void) | undefined
    const load = async () => {
      setLoading(true)
      try {
        const beam = await getBeam()
        const data = await fetchInventory(beam)
        const found = (data as any)?.currencies?.find?.((c: any) => c.id === currencyId)
        if (!cancelled) {
          setAmount(typeof found?.amount === "number" ? found.amount : null)
        }
      } catch (err) {
        console.warn("[Currency] Failed to fetch currencies:", err)
        if (!cancelled) setAmount(null)
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    load()

    ;(async () => {
      try {
        const beam = await getBeam()
        if (typeof (beam as any)?.on === "function") {
          const handler = () => load()
          ;(beam as any).on?.("inventory.refresh", handler)
          unsubscribe = () => {
            try {
              ;(beam as any).off?.("inventory.refresh", handler)
            } catch {}
          }
        }
      } catch {}
    })()

    return () => {
      cancelled = true
      try {
        unsubscribe?.()
      } catch {}
    }
  }, [readyForGame, refreshKey, currencyId])

  return { amount, loading }
}
