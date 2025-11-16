import type React from "react"
import { act, renderHook, type RenderHookResult } from "@testing-library/react"
import { beforeEach, afterEach, describe, expect, it, vi } from "vitest"
import useTowerGame from "@/hooks/useTowerGame"

const towersFixture = vi.hoisted(() => [
  {
    x: 500,
    y: 300,
    width: 30,
    height: 80,
    destroyed: false,
    color: "#FFFFFF",
    isSpecial: false,
    hits: 0,
    maxHits: 1,
  },
  {
    x: 600,
    y: 250,
    width: 40,
    height: 120,
    destroyed: false,
    color: "#AAAAAA",
    isSpecial: true,
    hits: 0,
    maxHits: 2,
  },
])

const mockParticles = vi.hoisted(() => ({
  createParticles: vi.fn(),
  createLaserParticles: vi.fn(),
  createLaserDestructionParticles: vi.fn(),
  createWinParticles: vi.fn(),
  createLoseParticles: vi.fn(),
}))

const mockAudio = vi.hoisted(() => ({
  playShootSound: vi.fn(),
  playGroundBounceSound: vi.fn(),
  playLaserShootSound: vi.fn(),
  playTowerBreakSound: vi.fn(),
  playWinSound: vi.fn(),
  playLoseSound: vi.fn(),
  playChargingSound: vi.fn(),
  stopChargingSound: vi.fn(),
  playRestartSound: vi.fn(),
  playStartSound: vi.fn(),
  playSelectSound: vi.fn(),
}))

vi.mock("@/components/Game/towers", () => ({
  generateTowers: vi.fn(() => ({
    towerCount: towersFixture.length,
    towers: towersFixture.map((tower, index) => ({
      ...tower,
      // ensure unique ids when the hook mutates towers
      hits: tower.hits ?? 0,
      destroyed: tower.destroyed ?? false,
      id: index,
    })),
  })),
}))

vi.mock("@/components/Game/particles", () => mockParticles)
vi.mock("@/components/Game/audio", () => mockAudio)

const createMockContext = () => ({
  fillStyle: "",
  strokeStyle: "",
  lineWidth: 1,
  beginPath: vi.fn(),
  moveTo: vi.fn(),
  lineTo: vi.fn(),
  arc: vi.fn(),
  fill: vi.fn(),
  stroke: vi.fn(),
  fillRect: vi.fn(),
  strokeRect: vi.fn(),
  setLineDash: vi.fn(),
  createLinearGradient: vi.fn(() => ({
    addColorStop: vi.fn(),
  })),
})

type TowerGameHookResult = ReturnType<typeof useTowerGame>

const attachCanvasToHook = (result: RenderHookResult<TowerGameHookResult, undefined>["result"]) => {
  const canvas = document.createElement("canvas")
  Object.defineProperty(canvas, "getContext", {
    value: vi.fn(() => createMockContext()),
  })
  Object.defineProperty(canvas, "getBoundingClientRect", {
    value: () =>
      ({
        left: 0,
        top: 0,
        width: 1200,
        height: 600,
      }) as DOMRect,
  })

  act(() => {
    const canvasRef = result.current.canvasRef as React.MutableRefObject<HTMLCanvasElement | null>
    canvasRef.current = canvas as HTMLCanvasElement
  })

  return canvas
}

describe("useTowerGame", () => {
  let rafSpy: ReturnType<typeof vi.spyOn>
  let cancelRafSpy: ReturnType<typeof vi.spyOn>

  beforeEach(() => {
    vi.clearAllMocks()
    vi.useFakeTimers()

    rafSpy = vi.spyOn(window, "requestAnimationFrame").mockImplementation(() => 1 as unknown as number)
    cancelRafSpy = vi.spyOn(window, "cancelAnimationFrame").mockImplementation(() => {})

    Object.defineProperty(HTMLCanvasElement.prototype, "getContext", {
      value: vi.fn(() => createMockContext()),
      configurable: true,
    })
  })

  afterEach(() => {
    rafSpy.mockRestore()
    cancelRafSpy.mockRestore()
    vi.useRealTimers()
  })

  it("allows selecting a different ball type", () => {
    const { result } = renderHook<TowerGameHookResult, undefined>(() => useTowerGame({ readyForGame: true }))

    expect(result.current.selectedBallType).toBe("normal")

    act(() => {
      result.current.selectBallType("fire")
    })

    expect(result.current.selectedBallType).toBe("fire")
    expect(mockAudio.playSelectSound).toHaveBeenCalled()
  })

  it("marks the game as started when startFirstShot is invoked", () => {
    const { result } = renderHook<TowerGameHookResult, undefined>(() => useTowerGame({ readyForGame: true }))

    expect(result.current.hasShot).toBe(false)

    act(() => {
      result.current.startFirstShot()
    })

    expect(result.current.hasShot).toBe(true)
    expect(mockAudio.playStartSound).toHaveBeenCalled()
  })

  it("handles pointer interactions to shoot a ball when ready", () => {
    const { result } = renderHook<TowerGameHookResult, undefined>(() => useTowerGame({ readyForGame: true }))
    attachCanvasToHook(result)

    act(() => {
      result.current.startFirstShot()
    })

    const initialBalls = result.current.ballsLeft

    act(() => {
      result.current.handlePointerDown({ clientX: 400, clientY: 200 } as React.PointerEvent<HTMLCanvasElement>)
    })

    expect(result.current.isCharging).toBe(true)

    act(() => {
      result.current.handlePointerUp({ clientX: 450, clientY: 180 } as React.PointerEvent<HTMLCanvasElement>)
    })

    expect(result.current.isCharging).toBe(false)
    expect(result.current.ballsLeft).toBe(initialBalls - 1)
    expect(mockAudio.playShootSound).toHaveBeenCalled()
    expect(mockAudio.stopChargingSound).toHaveBeenCalled()
  })
})
