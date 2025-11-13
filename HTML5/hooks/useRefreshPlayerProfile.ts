"use client"

import { useCallback } from "react"
import {
  initBeamPlayer,
  fetchPlayerAlias,
  fetchStellarIdentityInfo,
} from "@/lib/beam/player"

type RefreshArgs = {
  stellarLoggedOnceRef: React.MutableRefObject<boolean>
  setPlayerId: React.Dispatch<React.SetStateAction<string | null>>
  setAlias: React.Dispatch<React.SetStateAction<string | null>>
  setAliasInput: React.Dispatch<React.SetStateAction<string>>
  setAliasModalOpen: React.Dispatch<React.SetStateAction<boolean>>
  setShowPlayerInfo: React.Dispatch<React.SetStateAction<boolean>>
  setStellarExternalId: React.Dispatch<React.SetStateAction<string | null>>
  setStellarExternalIdentityId: React.Dispatch<React.SetStateAction<string | null>>
}

export default function useRefreshPlayerProfile({
  stellarLoggedOnceRef,
  setPlayerId,
  setAlias,
  setAliasInput,
  setAliasModalOpen,
  setShowPlayerInfo,
  setStellarExternalId,
  setStellarExternalIdentityId,
}: RefreshArgs) {
  return useCallback(async () => {
    try {
      const [{ playerId: refreshedId }, aliasValue, identity] = await Promise.all([
        initBeamPlayer(),
        fetchPlayerAlias().catch(() => null),
        fetchStellarIdentityInfo().catch(() => null),
      ])
      stellarLoggedOnceRef.current = false
      setPlayerId(refreshedId ?? null)
      const normalizedAlias = aliasValue && aliasValue.length > 0 ? aliasValue : null
      setAlias(normalizedAlias)
      setAliasInput(normalizedAlias ?? "")
      const hasAlias = !!normalizedAlias
      setAliasModalOpen(!hasAlias)
      setShowPlayerInfo(hasAlias)
      if (identity) {
        setStellarExternalId(identity.custodialId)
        setStellarExternalIdentityId(identity.externalId)
      } else {
        setStellarExternalId(null)
        setStellarExternalIdentityId(null)
      }
    } catch (err) {
      console.error("[Stellar] Failed to refresh player profile:", err)
    }
  }, [
    setAlias,
    setAliasInput,
    setAliasModalOpen,
    setPlayerId,
    setShowPlayerInfo,
    setStellarExternalId,
    setStellarExternalIdentityId,
    stellarLoggedOnceRef,
  ])
}
