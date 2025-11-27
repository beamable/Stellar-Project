"use client"

import type { Beam, ListingContent, StoreContent } from "beamable-sdk"
import getBeam from "@/lib/beam"

const DEFAULT_STORE_CONTENT_ID = process.env.NEXT_PUBLIC_STORE_CONTENT_ID || "stores.Store_Nf"
const DEFAULT_MANIFEST_ID = "global"

type ResolvedStore = {
  store: StoreContent
  listings: ListingContent[]
}

export type ResolvedStoreResult = ResolvedStore & { fromCache: boolean }

let cachedStore: ResolvedStore | null = null
let inFlight: Promise<ResolvedStoreResult> | null = null

const getContentService = (beam: Beam) => {
  const service = (beam as any).content ?? (beam as any).clientServices?.content
  if (!service?.getById || !service?.getByIds) {
    throw new Error("Beam ContentService is unavailable. Ensure clientServices(beam) has been registered.")
  }
  return service as {
    getById: (params: { id: string; manifestId?: string }) => Promise<StoreContent>
    getByIds: (params: { ids: string[]; manifestId?: string }) => Promise<ListingContent[]>
  }
}

function normalizeContentId(id?: string | null): string {
  const trimmed = (id || "").trim()
  if (!trimmed) return DEFAULT_STORE_CONTENT_ID
  return trimmed
}

function normalizeManifestId(manifestId?: string | null) {
  const trimmed = (manifestId || "").trim()
  return trimmed.length > 0 ? trimmed : DEFAULT_MANIFEST_ID
}

export function resetCommerceCache() {
  cachedStore = null
  inFlight = null
}

export function getCachedStore(): ResolvedStore | null {
  return cachedStore
}

export async function resolveStoreContent(opts: {
  storeContentId?: string
  manifestId?: string
  forceRefresh?: boolean
} = {}): Promise<ResolvedStoreResult> {
  const storeContentId = normalizeContentId(opts.storeContentId)
  const manifestId = normalizeManifestId(opts.manifestId)

  if (opts.forceRefresh) {
    resetCommerceCache()
  }

  if (cachedStore && cachedStore.store?.id === storeContentId) {
    const resolved = { ...cachedStore, fromCache: true }
    console.log("[Commerce] Store resolved from cache:", resolved)
    return resolved
  }

  if (inFlight) {
    return inFlight
  }

  inFlight = (async () => {
    const beam = await getBeam()
    const content = getContentService(beam as Beam)

    const store = await content.getById({
      id: storeContentId,
      manifestId,
    })

    const listingIds = store?.properties?.listings?.links ?? []
    const listings =
      listingIds.length > 0
        ? await content.getByIds({
            ids: listingIds,
            manifestId,
          })
        : []

    const sortedListings = [...listings].sort((a, b) => {
      const priceA = (a as any)?.properties?.price?.data?.amount ?? Number.POSITIVE_INFINITY
      const priceB = (b as any)?.properties?.price?.data?.amount ?? Number.POSITIVE_INFINITY
      return priceA - priceB
    })

    const resolved: ResolvedStore = { store, listings: sortedListings }
    cachedStore = resolved
    const result: ResolvedStoreResult = { ...resolved, fromCache: false }
    console.log("[Commerce] Store resolved:", result)
    return result
  })()

  try {
    return await inFlight
  } finally {
    inFlight = null
  }
}

export async function initCommerceManager(options?: {
  storeContentId?: string
  manifestId?: string
  forceRefresh?: boolean
}): Promise<ResolvedStoreResult> {
  return resolveStoreContent(options)
}
