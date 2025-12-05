"use client"

import { useEffect, useState } from "react"
import type { ListingContent, StoreContent } from "beamable-sdk"
import { initCommerceManager, resetCommerceCache } from "@/lib/commerceManager"
import { debugLog } from "@/lib/debugLog"

type UseCommerceManagerOptions = {
  enabled: boolean
  refreshKey?: number
  storeContentId?: string
  manifestId?: string
}

type CommerceState = {
  store: StoreContent | null
  listings: ListingContent[]
  loading: boolean
  error: string | null
}

const initialState: CommerceState = {
  store: null,
  listings: [],
  loading: false,
  error: null,
}

export default function useCommerceManager({
  enabled,
  refreshKey = 0,
  storeContentId,
  manifestId,
}: UseCommerceManagerOptions): CommerceState {
  const [state, setState] = useState<CommerceState>(initialState)

  useEffect(() => {
    if (!enabled) {
      return
    }

    let cancelled = false
    Promise.resolve().then(() => {
      if (cancelled) return
      setState((prev) => ({ ...prev, loading: true, error: null }))
    })

    ;(async () => {
      try {
        if (refreshKey > 0) {
          resetCommerceCache()
        }
        const resolved = await initCommerceManager({
          storeContentId,
          manifestId,
          forceRefresh: refreshKey > 0,
        })
        if (cancelled) return
        setState({
          store: resolved.store,
          listings: resolved.listings,
          loading: false,
          error: null,
        })
        debugLog("[Commerce] Resolved store content (hook):", resolved)
      } catch (err) {
        if (cancelled) return
        setState({
          store: null,
          listings: [],
          loading: false,
          error: (err as any)?.message ?? "Failed to resolve store content",
        })
        console.warn("[Commerce] Failed to resolve store content:", err)
      }
    })()

    return () => {
      cancelled = true
    }
  }, [enabled, refreshKey, storeContentId, manifestId])

  if (!enabled) {
    return initialState
  }

  return state
}
