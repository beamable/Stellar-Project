import { beforeEach, describe, expect, it, vi } from "vitest"
import { getKnownBalls, resetBallContentCache, resolveBallContent } from "../beamContent"

const mockGetByType = vi.fn()
const mockBeam = { content: { getByType: mockGetByType } }

vi.mock("../beam", () => ({
  __esModule: true,
  default: vi.fn(() => Promise.resolve(mockBeam)),
}))

describe("beamContent", () => {
  beforeEach(() => {
    resetBallContentCache()
    mockGetByType.mockReset()
  })

  it("fetches and caches ball content", async () => {
    const sample = [
      {
        id: "ball.normal",
        version: "1",
        uri: "/content/ball.normal",
        name: "Normal Ball",
        description: "Baseline projectile",
        customProperties: { color: "#ffffff" },
      },
    ]

    mockGetByType.mockResolvedValue(sample)

    const first = await resolveBallContent()
    expect(first).toEqual(sample)
    expect(mockGetByType).toHaveBeenCalledWith({ type: "ball" })

    mockGetByType.mockClear()
    const second = await resolveBallContent()

    expect(second).toBe(first)
    expect(mockGetByType).not.toHaveBeenCalled()
    expect(getKnownBalls()).toEqual(sample)
  })
})
