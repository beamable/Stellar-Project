"use client"

import { describe, expect, it, vi, beforeEach } from "vitest"
import type { Beam, ListingContent, StoreContent } from "beamable-sdk"
import { resolveStoreContent, resetCommerceCache } from "@/lib/commerceManager"

type MockBeam = Beam & {
  content: {
    getById: (args: { id: string; manifestId?: string }) => Promise<StoreContent>
    getByIds: (args: { ids: string[]; manifestId?: string }) => Promise<ListingContent[]>
  }
}

const makeBeam = (stores: Record<string, StoreContent>, listings: Record<string, ListingContent>): MockBeam => {
  return {
    content: {
      async getById({ id }: { id: string; manifestId?: string }) {
        if (!stores[id]) throw new Error("store missing")
        return stores[id]
      },
      async getByIds({ ids }: { ids: string[]; manifestId?: string }) {
        return ids.map((id: string) => {
          const listing = listings[id]
          if (!listing) throw new Error(`listing ${id} missing`)
          return listing
        })
      },
    },
  } as unknown as MockBeam
}

const getBeamMock = vi.hoisted(() => vi.fn())

vi.mock("@/lib/beam", () => ({
  __esModule: true,
  default: getBeamMock,
  getBeam: getBeamMock,
}))

const mockStore = (id: string, listingIds: string[] = []): StoreContent =>
  ({
    id,
    properties: { listings: { links: listingIds } },
  }) as StoreContent

const mockListing = (id: string, amount: number): ListingContent =>
  ({
    id,
    properties: { price: { data: { amount } } },
  }) as ListingContent

describe("resolveStoreContent caching", () => {
  beforeEach(() => {
    resetCommerceCache()
    getBeamMock.mockReset()
  })

  it("returns cached result for same store + manifest", async () => {
    const stores = {
      "store-1": mockStore("store-1", ["listing-a", "listing-b"]),
    }
    const listings = {
      "listing-a": mockListing("listing-a", 5),
      "listing-b": mockListing("listing-b", 10),
    }
    getBeamMock.mockResolvedValue(makeBeam(stores, listings))

    const first = await resolveStoreContent({ storeContentId: "store-1", manifestId: "m1" })
    const second = await resolveStoreContent({ storeContentId: "store-1", manifestId: "m1" })

    expect(first.fromCache).toBe(false)
    expect(second.fromCache).toBe(true)
    expect(getBeamMock).toHaveBeenCalledTimes(1)
  })

  it("separates cache entries by manifest and store id", async () => {
    const stores = {
      "store-1": mockStore("store-1", []),
      "store-2": mockStore("store-2", []),
    }
    const listings: Record<string, ListingContent> = {}
    getBeamMock.mockResolvedValue(makeBeam(stores, listings))

    const a = await resolveStoreContent({ storeContentId: "store-1", manifestId: "m1" })
    const b = await resolveStoreContent({ storeContentId: "store-1", manifestId: "m2" })
    const c = await resolveStoreContent({ storeContentId: "store-2", manifestId: "m1" })

    expect(a.fromCache).toBe(false)
    expect(b.fromCache).toBe(false)
    expect(c.fromCache).toBe(false)
    expect(getBeamMock).toHaveBeenCalledTimes(3)
  })

  it("forceRefresh bypasses cache and updates entry", async () => {
    const storesV1 = {
      "store-1": mockStore("store-1", ["listing-a"]),
    }
    const listingsV1 = {
      "listing-a": mockListing("listing-a", 5),
    }
    const storesV2 = {
      "store-1": mockStore("store-1", ["listing-b"]),
    }
    const listingsV2 = {
      "listing-b": mockListing("listing-b", 7),
    }

    getBeamMock.mockResolvedValue(makeBeam(storesV1, listingsV1))
    const first = await resolveStoreContent({ storeContentId: "store-1", manifestId: "m1" })
    expect(first.listings.map((l) => l.id)).toEqual(["listing-a"])
    expect(first.fromCache).toBe(false)

    getBeamMock.mockResolvedValue(makeBeam(storesV2, listingsV2))
    const second = await resolveStoreContent({ storeContentId: "store-1", manifestId: "m1", forceRefresh: true })
    expect(second.listings.map((l) => l.id)).toEqual(["listing-b"])
    expect(second.fromCache).toBe(false)
    expect(getBeamMock).toHaveBeenCalledTimes(2)
  })
})
