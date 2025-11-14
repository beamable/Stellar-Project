import { act, renderHook, waitFor } from "@testing-library/react"
import { beforeEach, describe, expect, it, vi } from "vitest"
import useBeamIdentity from "@/hooks/useBeamIdentity"

const refreshMockContainer = vi.hoisted(() => ({
  refresh: vi.fn(),
}))

const beamPlayerMocks = vi.hoisted(() => ({
  initBeamPlayerMock: vi.fn(),
  saveAliasAndAttachWalletMock: vi.fn(),
  fetchStellarIdentityInfoMock: vi.fn(),
}))

vi.mock("@/hooks/useRefreshPlayerProfile", () => ({
  __esModule: true,
  default: vi.fn(() => refreshMockContainer.refresh),
}))

vi.mock("@/lib/beam/player", () => ({
  initBeamPlayer: beamPlayerMocks.initBeamPlayerMock,
  saveAliasAndAttachWallet: beamPlayerMocks.saveAliasAndAttachWalletMock,
  fetchStellarIdentityInfo: beamPlayerMocks.fetchStellarIdentityInfoMock,
}))

describe("useBeamIdentity", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    beamPlayerMocks.initBeamPlayerMock.mockResolvedValue({ playerId: "player-42" })
    beamPlayerMocks.saveAliasAndAttachWalletMock.mockResolvedValue({ stellarId: null })
    beamPlayerMocks.fetchStellarIdentityInfoMock.mockResolvedValue({ custodialId: null, externalId: null })
    refreshMockContainer.refresh.mockResolvedValue(undefined)
  })

  it("initializes the Beam player and refreshes the profile once ready", async () => {
    const { result } = renderHook(() => useBeamIdentity())

    await waitFor(() => {
      expect(result.current.beamReady).toBe(true)
    })

    expect(result.current.playerId).toBe("player-42")
    expect(refreshMockContainer.refresh).toHaveBeenCalledTimes(1)
  })

  it("filters alias input and exposes a valid save flag", async () => {
    const { result } = renderHook(() => useBeamIdentity())

    await waitFor(() => expect(result.current.beamReady).toBe(true))

    act(() => {
      result.current.handleAliasInputChange("Abc123!@#")
    })

    expect(result.current.aliasInput).toBe("Abc")
    expect(result.current.aliasCanSave).toBe(true)
  })

  it("prevents saving when the alias does not meet validation", async () => {
    const { result } = renderHook(() => useBeamIdentity())

    await waitFor(() => expect(result.current.beamReady).toBe(true))

    await act(async () => {
      await result.current.handleAliasSave()
    })

    expect(beamPlayerMocks.saveAliasAndAttachWalletMock).not.toHaveBeenCalled()
    expect(result.current.aliasError).toBe("Alias must be letters only, at least 3 characters.")
  })

  it("saves a valid alias and updates the identity state", async () => {
    beamPlayerMocks.saveAliasAndAttachWalletMock.mockResolvedValue({ stellarId: "stellar-99" })
    const { result } = renderHook(() => useBeamIdentity())

    await waitFor(() => expect(result.current.beamReady).toBe(true))

    act(() => {
      result.current.handleAliasInputChange("Hero")
    })

    await act(async () => {
      await result.current.handleAliasSave()
    })

    expect(beamPlayerMocks.saveAliasAndAttachWalletMock).toHaveBeenCalledWith("Hero")
    expect(result.current.alias).toBe("Hero")
    expect(result.current.aliasModalOpen).toBe(false)
    expect(result.current.showPlayerInfo).toBe(true)
  })
})
