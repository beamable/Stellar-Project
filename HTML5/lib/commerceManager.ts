"use client"

import type { Beam, ListingContent, StoreContent } from "beamable-sdk"
import getBeam from "@/lib/beam"
import { debugLog } from "@/lib/debugLog"

const DEFAULT_STORE_CONTENT_ID = process.env.NEXT_PUBLIC_STORE_CONTENT_ID || "stores.Store_Nf"
const DEFAULT_MANIFEST_ID = "global"

type ResolvedStore = {
  store: StoreContent
  listings: ListingContent[]
}

export type ResolvedStoreResult = ResolvedStore & { fromCache: boolean }

const cacheKeyFor = (storeContentId: string, manifestId: string) => `${storeContentId}::${manifestId}`
const cachedStores = new Map<string, ResolvedStore>()
const inflightStores = new Map<string, Promise<ResolvedStoreResult>>()

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
  cachedStores.clear()
  inflightStores.clear()
}

export function getCachedStore(options?: { storeContentId?: string; manifestId?: string }): ResolvedStore | null {
  const storeContentId = normalizeContentId(options?.storeContentId)
  const manifestId = normalizeManifestId(options?.manifestId)
  const key = cacheKeyFor(storeContentId, manifestId)
  return cachedStores.get(key) ?? null
}

export async function resolveStoreContent(opts: {
  storeContentId?: string
  manifestId?: string
  forceRefresh?: boolean
} = {}): Promise<ResolvedStoreResult> {
  const storeContentId = normalizeContentId(opts.storeContentId)
  const manifestId = normalizeManifestId(opts.manifestId)
  const key = cacheKeyFor(storeContentId, manifestId)

  if (opts.forceRefresh) {
    cachedStores.delete(key)
    inflightStores.delete(key)
  }

  const cached = cachedStores.get(key)
  if (cached) {
    const resolved = { ...cached, fromCache: true }
    debugLog("[Commerce] Store resolved from cache:", resolved)
    return resolved
  }

  const inflight = inflightStores.get(key)
  if (inflight) {
    return inflight
  }

  const requestPromise = (async () => {
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
    cachedStores.set(key, resolved)
    const result: ResolvedStoreResult = { ...resolved, fromCache: false }
    debugLog("[Commerce] Store resolved:", result)
    return result
  })()

  inflightStores.set(key, requestPromise)

  try {
    return await requestPromise
  } finally {
    inflightStores.delete(key)
  }
}

export async function initCommerceManager(options?: {
  storeContentId?: string
  manifestId?: string
  forceRefresh?: boolean
}): Promise<ResolvedStoreResult> {
  return resolveStoreContent(options)
}
