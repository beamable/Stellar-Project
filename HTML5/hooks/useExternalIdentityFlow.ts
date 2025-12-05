"use client"

import { useCallback, useEffect, useRef } from "react"
import {
  buildWalletConnectUrl,
  completeExternalIdentityChallenge,
  EXTERNAL_AUTH_CONTEXT,
  EXTERNAL_SIGN_CONTEXT,
  requestExternalIdentityChallenge,
  subscribeToExternalContext,
  type ExternalAddressSubscription,
} from "@/lib/beam/player"
import { debugLog } from "@/lib/debugLog"

function formatSignatureErrorMessage(err: unknown): string {
  const rawMessage =
    typeof (err as any)?.message === "string"
      ? (err as any).message
      : typeof err === "string"
        ? err
        : (() => {
            try {
              return JSON.stringify(err)
            } catch {
              return "Unknown error"
            }
          })()

  const lower = rawMessage.toLowerCase()
  const beamErrorCode = typeof err === "object" && err !== null ? (err as any).error : undefined

  if (lower.includes("external identity is unavailable") || beamErrorCode === "ExternalIdentityUnavailable") {
    return "The connected Stellar Wallet is already attached to another Beamable account. Please try again with a different wallet."
  }

  return rawMessage || "Stellar signature failed. Please try again."
}

type WalletBridgeDeps = {
  pendingSignUrl: string | null
  setPendingSignUrl: (value: string | null) => void
  signatureError: string | null
  setSignatureError: (value: string | null) => void
  blockedState: { blocked: boolean; url: string | null; context?: string | null }
  acknowledgeUserAction: () => void
  clearBlockedState: () => void
  openWalletWindow: (url: string, context?: string, opts?: { allowNew?: boolean }) => void
  primeWalletWindow: () => Window | null
  closeWalletWindow: () => void
  resetWalletBridge: () => void
}

type ExternalIdentityFlowOptions = {
  playerId: string | null
  aliasModalOpen: boolean
  refreshPlayerProfile: () => Promise<void>
  walletBridge: WalletBridgeDeps
}

export function useExternalIdentityFlow({
  playerId,
  aliasModalOpen,
  refreshPlayerProfile,
  walletBridge,
}: ExternalIdentityFlowOptions) {
  const {
    pendingSignUrl,
    setPendingSignUrl,
    signatureError,
    setSignatureError,
    blockedState,
    acknowledgeUserAction,
    clearBlockedState,
    openWalletWindow,
    primeWalletWindow,
    closeWalletWindow,
    resetWalletBridge,
  } = walletBridge

  const blocked = blockedState?.blocked ?? false
  const blockedUrl = blockedState?.url ?? null
  const blockedContext = blockedState?.context ?? null

  const externalAddressSubRef = useRef<ExternalAddressSubscription | null>(null)
  const externalSignatureSubRef = useRef<ExternalAddressSubscription | null>(null)
  const challengeSolutionRef = useRef<{ challenge_token?: string } | null>(null)
  const walletConnectUrlRef = useRef<string | null>(null)

  const buildSignUrlFromChallenge = useCallback((challengeToken: string) => {
    const base = walletConnectUrlRef.current
    if (!base) return null
    try {
      if (typeof window === "undefined" || typeof window.atob !== "function") {
        console.warn("[Stellar] Cannot decode challenge token without window.atob")
        return null
      }
      const [challengePayload] = String(challengeToken || "").split(".")
      if (!challengePayload) return null
      const decoded = window.atob(challengePayload)
      const encoded = encodeURIComponent(decoded)
      return `${base}&message=${encoded}`
    } catch (err) {
      console.warn("[Stellar] Failed to parse challenge token:", err)
      return null
    }
  }, [])

  const stopSubscriptions = useCallback(() => {
    try {
      externalAddressSubRef.current?.stop?.()
    } catch {}
    externalAddressSubRef.current = null
    try {
      externalSignatureSubRef.current?.stop?.()
    } catch {}
    externalSignatureSubRef.current = null
  }, [])

  const handleAttachExternalId = useCallback(() => {
    setSignatureError(null)
    acknowledgeUserAction()
    if (pendingSignUrl) {
      openWalletWindow(pendingSignUrl, "challenge signing", { allowNew: true })
      return
    }
    setPendingSignUrl(null)
    const primedWindow = primeWalletWindow()
    ;(async () => {
      try {
        const { url } = await buildWalletConnectUrl(playerId || null)
        walletConnectUrlRef.current = url
        debugLog("[Stellar] Launching wallet flow:", url)
        if (primedWindow && !primedWindow.closed) {
          primedWindow.location.href = url
          primedWindow.focus?.()
          acknowledgeUserAction()
          debugLog("[Stellar] Wallet window navigated for initial wallet connect.")
        } else {
          openWalletWindow(url, "initial wallet connect")
        }
        stopSubscriptions()
        const handleAddress = async (payload: any) => {
          try {
            debugLog("[Stellar] ExternalAuthAddress message payload:", payload)
            if (payload?.messageFull) {
              debugLog("[Stellar] ExternalAuthAddress raw messageFull:", payload.messageFull)
            }
            const ctxRaw = (payload && (payload.Context ?? payload.context)) || null
            const ctx = ctxRaw ? String(ctxRaw).toLowerCase() : null
            if (ctx && ctx !== EXTERNAL_AUTH_CONTEXT) {
              debugLog("[Stellar] Ignoring message for different context:", ctxRaw)
              return
            }
            let value = (payload && (payload.Value ?? payload.value)) || null
            if (!value && typeof payload?.messageFull === "string") {
              try {
                const inner = JSON.parse(payload.messageFull)
                value = inner?.Value ?? inner?.value ?? null
              } catch {}
            }
            if (!value || typeof value !== "string") {
              console.warn("[Stellar] ExternalAuthAddress payload missing Value:", payload)
              return
            }
            const challengeResp: any = await requestExternalIdentityChallenge(value)
            const challengeToken =
              challengeResp?.challenge_token || challengeResp?.challengeResponse?.challenge_token
            if (!challengeToken) {
              console.warn("[Stellar] No challenge_token returned from request:", challengeResp)
              return
            }
            challengeSolutionRef.current = { challenge_token: challengeToken }
            debugLog("[Stellar] challenge_token:", challengeToken)
            const signUrl = buildSignUrlFromChallenge(challengeToken)
            debugLog("[Stellar] Built sign URL from challenge:", signUrl || "[none]")
            if (signUrl) {
              setPendingSignUrl(signUrl)
              debugLog('[Stellar] Stellar bridge ready. Click "Sign Stellar Wallet" to continue.')
            } else {
              console.warn("[Stellar] Unable to build sign URL - missing wallet bridge base or invalid payload.")
            }
          } catch (err) {
            console.error("[Stellar] External ID challenge request error:", (err as any)?.message || err)
          } finally {
            stopSubscriptions()
            debugLog("[Stellar] ExternalAuthAddress subscription stopped after challenge request cycle.")
          }
        }
        const handleSignature = async (payload: any) => {
          try {
            debugLog("[Stellar] ExternalAuthSignature message payload:", payload)
            let signature = (payload && (payload.Value ?? payload.value)) || null
            if (!signature && typeof payload?.messageFull === "string") {
              try {
                const inner = JSON.parse(payload.messageFull)
                signature = inner?.Value ?? inner?.value ?? null
              } catch {}
            }
            if (!signature || typeof signature !== "string") {
              console.warn("[Stellar] ExternalAuthSignature payload missing Value:", payload)
              return
            }
            const challengeToken = challengeSolutionRef.current?.challenge_token
            if (!challengeToken) {
              console.warn("[Stellar] Missing challenge_token when signature arrived")
              return
            }
            await completeExternalIdentityChallenge(challengeToken, signature)
            await refreshPlayerProfile()
            debugLog("[Stellar] External identity attached via signature.")
            closeWalletWindow()
            setPendingSignUrl(null)
            setSignatureError(null)
            clearBlockedState()
            stopSubscriptions()
            debugLog("[Stellar] ExternalAuthSignature subscription stopped after successful attachment.")
          } catch (err) {
            const message = formatSignatureErrorMessage(err)
            setSignatureError(message || "Stellar signature failed. Please try again.")
            console.error("[Stellar] External signature flow error:", (err as any)?.message || err)
          }
        }
        externalAddressSubRef.current = await subscribeToExternalContext(EXTERNAL_AUTH_CONTEXT, handleAddress, {
          intervalMs: 2000,
        })
        debugLog("[Stellar] Subscribed to ExternalAuthAddress notifications.")
        externalSignatureSubRef.current = await subscribeToExternalContext(EXTERNAL_SIGN_CONTEXT, handleSignature, {
          intervalMs: 2000,
        })
        debugLog("[Stellar] Subscribed to ExternalAuthSignature notifications.")
      } catch (e) {
        console.error("[Stellar] Failed to open External ID attach flow:", (e as any)?.message || e)
      }
    })()
  }, [
    acknowledgeUserAction,
    clearBlockedState,
    closeWalletWindow,
    openWalletWindow,
    pendingSignUrl,
    playerId,
    primeWalletWindow,
    refreshPlayerProfile,
    setPendingSignUrl,
    setSignatureError,
    stopSubscriptions,
  ])

  const handleRetryAttach = useCallback(() => {
    setSignatureError(null)
    setPendingSignUrl(null)
    resetWalletBridge()
    closeWalletWindow()
  }, [closeWalletWindow, resetWalletBridge, setPendingSignUrl, setSignatureError])

  const handleManualWalletOpen = useCallback(() => {
    if (!blockedUrl) {
      return
    }
    acknowledgeUserAction()
    openWalletWindow(blockedUrl, blockedContext || "Stellar wallet", { allowNew: true })
  }, [acknowledgeUserAction, blockedContext, blockedUrl, openWalletWindow])

  useEffect(() => {
    return () => {
      stopSubscriptions()
      closeWalletWindow()
    }
  }, [closeWalletWindow, stopSubscriptions])

  useEffect(() => {
    if (aliasModalOpen) {
      return
    }
    stopSubscriptions()
    setPendingSignUrl(null)
    setSignatureError(null)
    resetWalletBridge()
  }, [aliasModalOpen, resetWalletBridge, setPendingSignUrl, setSignatureError, stopSubscriptions])

  return {
    pendingSignUrl,
    signatureError,
    walletPopupBlocked: blocked,
    walletPopupBlockedUrl: blockedUrl,
    walletPopupContext: blockedContext,
    handleAttachExternalId,
    handleRetryAttach,
    handleManualWalletOpen,
  }
}

export default useExternalIdentityFlow
