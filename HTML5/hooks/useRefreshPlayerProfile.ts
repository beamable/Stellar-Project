"use client"

import { useCallback, type MutableRefObject, type Dispatch, type SetStateAction } from "react"
import {
  initBeamPlayer,
  fetchPlayerAlias,
  fetchStellarIdentityInfo,
} from "@/lib/beam/player"

type RefreshArgs = {
  stellarLoggedOnceRef: MutableRefObject<boolean>
  setPlayerId: Dispatch<SetStateAction<string | null>>
  setAlias: Dispatch<SetStateAction<string | null>>
  setAliasInput: Dispatch<SetStateAction<string>>
  setAliasModalOpen: Dispatch<SetStateAction<boolean>>
  setShowPlayerInfo: Dispatch<SetStateAction<boolean>>
  setStellarExternalId: Dispatch<SetStateAction<string | null>>
  setStellarExternalIdentityId: Dispatch<SetStateAction<string | null>>
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
