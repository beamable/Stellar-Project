"use client"

import { useCallback, useEffect, useRef, useState } from "react"
import useRefreshPlayerProfile from "@/hooks/useRefreshPlayerProfile"
import {
  initBeamPlayer,
  saveAliasAndAttachWallet,
  fetchStellarIdentityInfo,
} from "@/lib/beam/player"

type UseBeamIdentityResult = {
  beamReady: boolean
  playerId: string | null
  alias: string | null
  aliasInput: string
  aliasModalOpen: boolean
  aliasSaving: boolean
  aliasError: string | null
  aliasCanSave: boolean
  showPlayerInfo: boolean
  commandDeckSeen: boolean
  readyForGame: boolean
  stellarExternalId: string | null
  stellarExternalIdentityId: string | null
  handleAliasInputChange: (value: string) => void
  handleAliasSave: () => Promise<void>
  setShowPlayerInfo: (value: boolean) => void
  setCommandDeckSeen: (value: boolean) => void
  refreshPlayerProfile: () => Promise<void>
  debugFakeLogin: () => void
}

export default function useBeamIdentity(): UseBeamIdentityResult {
  const stellarLoggedOnceRef = useRef<boolean>(false)
  const [playerId, setPlayerId] = useState<string | null>(null)
  const [beamReady, setBeamReady] = useState(false)
  const [alias, setAlias] = useState<string | null>(null)
  const [aliasInput, setAliasInput] = useState("")
  const [aliasModalOpen, setAliasModalOpen] = useState(false)
  const [aliasSaving, setAliasSaving] = useState(false)
  const [aliasError, setAliasError] = useState<string | null>(null)
  const [showPlayerInfo, setShowPlayerInfoState] = useState(false)
  const [commandDeckSeen, setCommandDeckSeen] = useState(false)
  const [stellarExternalId, setStellarExternalId] = useState<string | null>(null)
  const [stellarExternalIdentityId, setStellarExternalIdentityId] = useState<string | null>(null)

  const handleAliasInputChange = useCallback((value: string) => {
    const filtered = value.replace(/[^A-Za-z]/g, "")
    setAliasInput(filtered)
  }, [])

  const setShowPlayerInfo = useCallback((value: boolean) => {
    setShowPlayerInfoState(value)
    if (value) {
      setCommandDeckSeen(true)
    }
  }, [])

  const aliasCanSave = /^[A-Za-z]{3,}$/.test(aliasInput)
  const readyForGame = beamReady && !!(alias && alias.length > 0)

  const refreshPlayerProfile = useRefreshPlayerProfile({
    stellarLoggedOnceRef,
    setPlayerId,
    setAlias,
    setAliasInput,
    setAliasModalOpen,
    setShowPlayerInfo,
    setStellarExternalId,
    setStellarExternalIdentityId,
  })

  const handleAliasSave = useCallback(async () => {
    setAliasError(null)
    if (!aliasCanSave) {
      setAliasError("Alias must be letters only, at least 3 characters.")
      return
    }
    setAliasSaving(true)
    try {
      const { stellarId } = await saveAliasAndAttachWallet(aliasInput)
      if (stellarId) {
        setStellarExternalId(stellarId)
      }
      setAlias(aliasInput)
      setAliasModalOpen(false)
      setShowPlayerInfo(true)
    } catch (e: any) {
      setAliasError(e?.message || "Failed to save alias. Try again.")
    } finally {
      setAliasSaving(false)
    }
  }, [aliasCanSave, aliasInput, setShowPlayerInfo])

  useEffect(() => {
    let mounted = true
    initBeamPlayer()
      .then(({ playerId: id }) => {
        if (!mounted) return
        setPlayerId(id)
        setBeamReady(true)
      })
      .catch((err: unknown) => {
        console.error("[Beam] Initialization failed:", (err as any)?.message || err)
      })
    return () => {
      mounted = false
    }
  }, [setShowPlayerInfo])

  useEffect(() => {
    if (!beamReady) return
    refreshPlayerProfile()
  }, [beamReady, refreshPlayerProfile])

  useEffect(() => {
    if (!beamReady) return
    if (!alias || alias.length === 0) return
    if (stellarLoggedOnceRef.current) return
    ;(async () => {
      try {
        const info = await fetchStellarIdentityInfo()
        if (info.custodialId) {
          setStellarExternalId(info.custodialId)
        }
        if (info.externalId) {
          setStellarExternalIdentityId(info.externalId)
        }
        stellarLoggedOnceRef.current = true
      } catch {}
    })()
  }, [beamReady, alias])

  const debugFakeLogin = useCallback(() => {
    setBeamReady(true)
    setPlayerId("DEV-PLAYER")
    setAlias("guest")
    setAliasInput("guest")
    setAliasModalOpen(false)
    setShowPlayerInfo(true)
    setStellarExternalId("CUSTODIAL-DEV")
    setStellarExternalIdentityId("EXTERNAL-DEV")
  }, [setShowPlayerInfo])

  return {
    beamReady,
    playerId,
    alias,
    aliasInput,
    aliasModalOpen,
    aliasSaving,
    aliasError,
    aliasCanSave,
    showPlayerInfo,
    commandDeckSeen,
    readyForGame,
    stellarExternalId,
    stellarExternalIdentityId,
    handleAliasInputChange,
    handleAliasSave,
    setShowPlayerInfo,
    setCommandDeckSeen,
    refreshPlayerProfile,
    debugFakeLogin,
  }
}
