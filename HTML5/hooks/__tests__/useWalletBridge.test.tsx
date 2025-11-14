import { act, renderHook } from "@testing-library/react"
import { afterEach, describe, expect, it, vi } from "vitest"
import useWalletBridge from "@/hooks/useWalletBridge"

const originalWindowOpen = window.open

describe("useWalletBridge", () => {
  afterEach(() => {
    vi.restoreAllMocks()
    window.open = originalWindowOpen
  })

  it("tracks pending sign URLs and clears blocked state", () => {
    const { result } = renderHook(() => useWalletBridge())

    act(() => {
      result.current.setPendingSignUrl("https://wallet.example/sign")
    })

    expect(result.current.pendingSignUrl).toBe("https://wallet.example/sign")
    expect(result.current.blockedState.blocked).toBe(false)
  })

  it("opens a wallet window when the browser allows it", () => {
    const mockWin = {
      closed: false,
      focus: vi.fn(),
      location: { href: "about:blank" },
    } as unknown as Window

    const openSpy = vi.spyOn(window, "open").mockReturnValue(mockWin)

    const { result } = renderHook(() => useWalletBridge())

    let opened: Window | null = null
    act(() => {
      opened = result.current.openWalletWindow("https://wallet.example/attach", "attach-flow")
    })

    expect(openSpy).toHaveBeenCalled()
    expect(opened).toBe(mockWin)
    expect(result.current.blockedState.blocked).toBe(false)
  })

  it("flags the popup as blocked when the browser refuses to open it", () => {
    vi.spyOn(window, "open").mockReturnValue(null)
    const { result } = renderHook(() => useWalletBridge())

    act(() => {
      result.current.openWalletWindow("https://wallet.example/attach", "attach-flow")
    })

    expect(result.current.blockedState.blocked).toBe(true)
    expect(result.current.blockedState.url).toBe("https://wallet.example/attach")
    expect(result.current.blockedState.context).toBe("attach-flow")
  })

  it("resets popup state and closes any open window", () => {
    const closeSpy = vi.fn()
    const mockWin = {
      closed: false,
      close: closeSpy,
      focus: vi.fn(),
      location: { href: "about:blank" },
    } as unknown as Window

    vi.spyOn(window, "open").mockReturnValue(mockWin)

    const { result } = renderHook(() => useWalletBridge())

    act(() => {
      result.current.setPendingSignUrl("https://wallet.example/sign")
      result.current.setSignatureError("boom")
      result.current.openWalletWindow("https://wallet.example/attach", "attach-flow")
    })

    act(() => {
      result.current.reset()
    })

    expect(result.current.pendingSignUrl).toBeNull()
    expect(result.current.signatureError).toBeNull()
    expect(closeSpy).toHaveBeenCalled()
  })
})
