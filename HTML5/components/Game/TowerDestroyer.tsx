"use client"

import type React from "react"
import { useState, useEffect, useRef, useCallback } from "react"
import useBeamIdentity from "@/hooks/useBeamIdentity"
import useWalletBridge from "@/hooks/useWalletBridge"
import GameShell from "@/components/Game/GameShell"
import { getMasterVolume, setMasterVolume } from "@/components/Game/audio"

const SHOULD_LOG_RENDERS = process.env.NEXT_PUBLIC_TD_RENDER_DEBUG === "true"
const VOLUME_STORAGE_KEY = "tower-destroyer-volume"
const IS_DEV = process.env.NODE_ENV !== "production"

function useRenderCounter(label: string, enabled: boolean) {
  const renderCountRef = useRef(0)
  renderCountRef.current += 1

  useEffect(() => {
    if (!enabled) {
      return
    }
    console.log(`[RenderCounter] ${label} render #${renderCountRef.current}`)
  })
}

// Import types
import * as CONST from "./constants"
import useTowerGame from "@/hooks/useTowerGame"
import useBallLoadout from "@/hooks/useBallLoadout"
import useCurrency from "@/hooks/useCurrency"
import useCommerceManager from "@/hooks/useCommerceManager"
import { CAMPAIGN_STAGES, CAMPAIGN_STAGE_MAP, DEFAULT_STAGE_ID } from "@/components/Game/campaign"
import useCampaignProgress from "@/hooks/useCampaignProgress"
import {
  resetBeamSession,
  buildWalletConnectUrl,
  subscribeToExternalContext,
  requestExternalIdentityChallenge,
  completeExternalIdentityChallenge,
  EXTERNAL_AUTH_CONTEXT,
  EXTERNAL_SIGN_CONTEXT,
} from "@/lib/beam/player"
import getBeam from "@/lib/beam"
import type { ExternalAddressSubscription } from "@/lib/beam/player"

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

export default function TowerDestroyer() {
  useRenderCounter("TowerDestroyer", SHOULD_LOG_RENDERS)
  const [showResetConfirm, setShowResetConfirm] = useState(false)
  const [volume, setVolume] = useState(1)
  const [showAudioSettings, setShowAudioSettings] = useState(false)
  const {
    beamReady,
    playerId,
    alias,
    aliasInput,
    aliasModalOpen,
    aliasSaving,
    aliasError,
    aliasCanSave,
    showPlayerInfo,
    setShowPlayerInfo,
    readyForGame,
    stellarExternalId,
    stellarExternalIdentityId,
    handleAliasInputChange,
    handleAliasSave,
    refreshPlayerProfile,
    debugFakeLogin,
  } = useBeamIdentity()

  const {
    stageProgress,
    selectedStage,
    selectStage,
    markStageComplete,
    pendingMechanics,
    acknowledgeMechanics,
    campaignComplete,
    loopCount,
    startNextLoop,
  } = useCampaignProgress(playerId)
  const activeStage = selectedStage ?? CAMPAIGN_STAGE_MAP.get(DEFAULT_STAGE_ID)!
  const totalStages = CAMPAIGN_STAGES.length
  const [inventoryRefreshKey, setInventoryRefreshKey] = useState(0)
  const { ballTypes, ballTypeMap, ownedBallTypes, loading: ballLoadoutLoading } = useBallLoadout(
    readyForGame,
    inventoryRefreshKey,
  )
  const { amount: currencyAmount, loading: currencyLoading } = useCurrency(readyForGame, inventoryRefreshKey)
  const inventoryInitialized = readyForGame && !ballLoadoutLoading && !currencyLoading
  const {
    store: storeContent,
    listings: storeListings,
    loading: commerceLoading,
    error: commerceError,
  } = useCommerceManager({
    enabled: inventoryInitialized,
    refreshKey: inventoryRefreshKey,
  })
  const [campaignUnlocked, setCampaignUnlocked] = useState(false)
  const coinsSyncedRef = useRef(false)
  const {
    canvasRef,
    selectedBallType,
    selectBallType,
    gameState,
    score,
    coinsEarned,
    ballsLeft,
    towerCount,
    remainingTowers,
    powerSnapshot,
    isCharging,
    hasShot,
    handlePointerDown,
    handlePointerMove,
    handlePointerUp,
    resetGame,
    startFirstShot,
    debugForceWin,
  } = useTowerGame({
    readyForGame,
    towerProfile: activeStage.towerProfile,
    stageId: activeStage.id,
    ballTypeMap,
  })
  const availableBallConfigs = ballTypes.filter((ball) => ownedBallTypes.includes(ball.type))
  useEffect(() => {
    if (!ownedBallTypes.includes(selectedBallType)) {
      const fallback = ownedBallTypes[0] ?? "normal"
      selectBallType(fallback)
    }
  }, [ownedBallTypes, selectedBallType, selectBallType])
  const selectedBallInfo = ballTypes.find((ball) => ball.type === selectedBallType)
  const stageLabel = `Stage ${activeStage.order + 1}/${totalStages}`
  const loopLabel = `Loop ${loopCount + 1}`
  const nextStage = CAMPAIGN_STAGES[activeStage.order + 1] ?? null
  const nextStageProgress = nextStage
    ? stageProgress.find((entry) => entry.stage.id === nextStage.id)
    : null
  const canAdvanceStage = Boolean(nextStage && nextStageProgress && nextStageProgress.status !== "locked")
  const {
    pendingSignUrl,
    setPendingSignUrl,
    signatureError,
    setSignatureError,
    blockedState,
    clearBlockedState,
    acknowledgeUserAction,
    openWalletWindow,
    primeWalletWindow,
    closeWalletWindow,
    reset: resetWalletBridge,
  } = useWalletBridge()
  const { blocked: walletPopupBlocked, url: walletPopupBlockedUrl, context: walletPopupContext } = blockedState
  // External auth address subscription handle
  const externalAddressSubRef = useRef<ExternalAddressSubscription | null>(null)
  const externalSignatureSubRef = useRef<ExternalAddressSubscription | null>(null)
  const campaignWinStageRef = useRef<string | null>(null)
  // ChallengeSolution that we will carry through the process
  const challengeSolutionRef = useRef<{ challenge_token?: string } | null>(null)
  const walletConnectUrlRef = useRef<string | null>(null)
  const buildSignUrlFromChallenge = useCallback(
    (challengeToken: string) => {
      const base = walletConnectUrlRef.current
      if (!base) return null
      try {
        if (typeof window === 'undefined' || typeof window.atob !== 'function') {
          console.warn('[Stellar] Cannot decode challenge token without window.atob')
          return null
        }
        const [challengePayload] = String(challengeToken || '').split('.')
        if (!challengePayload) return null
        const decoded = window.atob(challengePayload)
        const encoded = encodeURIComponent(decoded)
        return `${base}&message=${encoded}`
      } catch (err) {
        console.warn('[Stellar] Failed to parse challenge token:', err)
        return null
      }
    },
    [],
  )

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
        console.log("[Stellar] Launching wallet flow:", url)
        if (primedWindow && !primedWindow.closed) {
          primedWindow.location.href = url
          primedWindow.focus?.()
          acknowledgeUserAction()
          console.log("[Stellar] Wallet window navigated for initial wallet connect.")
        } else {
          openWalletWindow(url, "initial wallet connect")
        }
        externalAddressSubRef.current?.stop?.()
        externalAddressSubRef.current = null
        externalSignatureSubRef.current?.stop?.()
        externalSignatureSubRef.current = null
        const handleAddress = async (payload: any) => {
          try {
            console.log('[Stellar] ExternalAuthAddress message payload:', payload)
            if (payload?.messageFull) {
              console.log('[Stellar] ExternalAuthAddress raw messageFull:', payload.messageFull)
            }
            const ctxRaw = (payload && (payload.Context ?? payload.context)) || null
            const ctx = ctxRaw ? String(ctxRaw).toLowerCase() : null
            if (ctx && ctx !== EXTERNAL_AUTH_CONTEXT) {
              console.log('[Stellar] Ignoring message for different context:', ctxRaw)
              return
            }
            let value = (payload && (payload.Value ?? payload.value)) || null
            if (!value && typeof payload?.messageFull === 'string') {
              try {
                const inner = JSON.parse(payload.messageFull)
                value = inner?.Value ?? inner?.value ?? null
              } catch {}
            }
            if (!value || typeof value !== 'string') {
              console.warn('[Stellar] ExternalAuthAddress payload missing Value:', payload)
              return
            }
            const challengeResp: any = await requestExternalIdentityChallenge(value)
            const challengeToken =
              challengeResp?.challenge_token || challengeResp?.challengeResponse?.challenge_token
            if (!challengeToken) {
              console.warn('[Stellar] No challenge_token returned from request:', challengeResp)
              return
            }
            challengeSolutionRef.current = { challenge_token: challengeToken }
            console.log('[Stellar] challenge_token:', challengeToken)
            const signUrl = buildSignUrlFromChallenge(challengeToken)
            console.log('[Stellar] Built sign URL from challenge:', signUrl || '[none]')
            if (signUrl) {
              setPendingSignUrl(signUrl)
              console.log('[Stellar] Stellar bridge ready. Click "Sign Stellar Wallet" to continue.')
            } else {
              console.warn('[Stellar] Unable to build sign URL - missing wallet bridge base or invalid payload.')
            }
          } catch (err) {
            console.error('[Stellar] External ID challenge request error:', (err as any)?.message || err)
          } finally {
            externalAddressSubRef.current?.stop?.()
            externalAddressSubRef.current = null
            console.log('[Stellar] ExternalAuthAddress subscription stopped after challenge request cycle.')
          }
        }
        const handleSignature = async (payload: any) => {
          try {
            console.log('[Stellar] ExternalAuthSignature message payload:', payload)
            let signature = (payload && (payload.Value ?? payload.value)) || null
            if (!signature && typeof payload?.messageFull === 'string') {
              try {
                const inner = JSON.parse(payload.messageFull)
                signature = inner?.Value ?? inner?.value ?? null
              } catch {}
            }
            if (!signature || typeof signature !== 'string') {
              console.warn('[Stellar] ExternalAuthSignature payload missing Value:', payload)
              return
            }
            const challengeToken = challengeSolutionRef.current?.challenge_token
            if (!challengeToken) {
              console.warn('[Stellar] Missing challenge_token when signature arrived')
              return
            }
            await completeExternalIdentityChallenge(challengeToken, signature)
            await refreshPlayerProfile()
            console.log('[Stellar] External identity attached via signature.')
            closeWalletWindow()
            setPendingSignUrl(null)
            setSignatureError(null)
            clearBlockedState()
            externalSignatureSubRef.current?.stop?.()
            externalSignatureSubRef.current = null
            console.log('[Stellar] ExternalAuthSignature subscription stopped after successful attachment.')
          } catch (err) {
            const message = formatSignatureErrorMessage(err)
            setSignatureError(message)
            console.error('[Stellar] External signature flow error:', (err as any)?.message || err)
          }
        }
        externalAddressSubRef.current = await subscribeToExternalContext(EXTERNAL_AUTH_CONTEXT, handleAddress, {
          intervalMs: 2000,
        })
        console.log('[Stellar] Subscribed to ExternalAuthAddress notifications.')
        externalSignatureSubRef.current = await subscribeToExternalContext(EXTERNAL_SIGN_CONTEXT, handleSignature, {
          intervalMs: 2000,
        })
        console.log('[Stellar] Subscribed to ExternalAuthSignature notifications.')
      } catch (e) {
        console.error('[Stellar] Failed to open External ID attach flow:', (e as any)?.message || e)
      }
    })()
  }, [
    setSignatureError,
    pendingSignUrl,
    openWalletWindow,
    setPendingSignUrl,
    primeWalletWindow,
    playerId,
    acknowledgeUserAction,
    clearBlockedState,
    buildSignUrlFromChallenge,
    refreshPlayerProfile,
    closeWalletWindow,
  ])

  const handleRetryAttach = useCallback(() => {
    setSignatureError(null)
    setPendingSignUrl(null)
    clearBlockedState()
    closeWalletWindow()
  }, [clearBlockedState, closeWalletWindow, setPendingSignUrl, setSignatureError])

  const handleManualWalletOpen = useCallback(() => {
    if (!walletPopupBlockedUrl) {
      return
    }
    acknowledgeUserAction()
    openWalletWindow(walletPopupBlockedUrl, walletPopupContext || "Stellar wallet", { allowNew: true })
  }, [acknowledgeUserAction, openWalletWindow, walletPopupBlockedUrl, walletPopupContext])


  useEffect(() => {
    return () => {
      try {
        externalAddressSubRef.current?.stop?.()
      } catch {}
      try {
        externalSignatureSubRef.current?.stop?.()
      } catch {}
      closeWalletWindow()
    }
  }, [closeWalletWindow])

  useEffect(() => {
    if (aliasModalOpen) {
      return
    }
    try {
      externalAddressSubRef.current?.stop?.()
    } catch {}
    externalAddressSubRef.current = null
    try {
      externalSignatureSubRef.current?.stop?.()
    } catch {}
    externalSignatureSubRef.current = null
    resetWalletBridge()
  }, [aliasModalOpen, resetWalletBridge])

  useEffect(() => {
    if (aliasModalOpen) {
      return
    }
    try {
      externalAddressSubRef.current?.stop?.()
    } catch {}
    externalAddressSubRef.current = null
    try {
      externalSignatureSubRef.current?.stop?.()
    } catch {}
    externalSignatureSubRef.current = null
    setPendingSignUrl(null)
    setSignatureError(null)
  }, [aliasModalOpen, setPendingSignUrl, setSignatureError])

  const handleAcknowledgeMechanics = useCallback(() => {
    if (pendingMechanics.length === 0) return
    acknowledgeMechanics(pendingMechanics)
  }, [acknowledgeMechanics, pendingMechanics])
  useEffect(() => {
    if (!inventoryInitialized) return
    if (commerceLoading) return
    if (!storeContent) return
    console.log("[Commerce] Store resolved and ready:", {
      store: storeContent,
      listings: storeListings,
      error: commerceError,
    })
  }, [inventoryInitialized, commerceLoading, storeContent, storeListings, commerceError])

  const [campaignConfirmed, setCampaignConfirmed] = useState(false)
  const lastStageRef = useRef(activeStage.id)
  const commandDeckSeenRef = useRef(false)
  useEffect(() => {
    if (showPlayerInfo) {
      commandDeckSeenRef.current = true
    } else if (!showPlayerInfo && commandDeckSeenRef.current) {
      setCampaignUnlocked(true)
    }
  }, [showPlayerInfo])
  useEffect(() => {
    if (lastStageRef.current !== activeStage.id) {
      lastStageRef.current = activeStage.id
      setCampaignConfirmed(false)
    }
  }, [activeStage.id])
  useEffect(() => {
    if (!readyForGame) {
      setCampaignUnlocked(false)
      commandDeckSeenRef.current = false
      setCampaignConfirmed(false)
      coinsSyncedRef.current = false
    }
  }, [readyForGame])
  useEffect(() => {
    if (showPlayerInfo) {
      setInventoryRefreshKey((prev) => prev + 1)
    }
  }, [showPlayerInfo])

  useEffect(() => {
    if (gameState === "playing") {
      coinsSyncedRef.current = false
      return
    }
    if (!readyForGame) return
    if (coinsSyncedRef.current) return
    if (coinsEarned <= 0) return
    coinsSyncedRef.current = true
    ;(async () => {
      try {
        const beam = await getBeam()
        const client = (beam as any)?.stellarFederationClient
        if (client?.updateCurrency) {
          const payload = { currencyContentId: "currency.coins", amount: coinsEarned }
          console.log("[Coins] Syncing earned coins to server:", payload)
          await client.updateCurrency(payload)
          setInventoryRefreshKey((prev) => prev + 1)
        } else {
          console.warn("[Coins] StellarFederationClient.updateCurrency unavailable; skipping sync.")
        }
      } catch (err) {
        console.warn("[Coins] Failed to sync earned coins:", err)
      }
    })()
  }, [gameState, readyForGame, coinsEarned])
  const shouldShowCampaignOverlay =
    campaignUnlocked && readyForGame && !showPlayerInfo && !campaignConfirmed

  const handleConfirmCampaignStage = useCallback(() => {
    setCampaignConfirmed(true)
  }, [])

  const handleDebugSkipStage = useCallback(() => {
    if (!IS_DEV) return
    debugForceWin()
  }, [debugForceWin])

  const handleDebugFakeLogin = useCallback(() => {
    if (!IS_DEV) return
    debugFakeLogin()
    commandDeckSeenRef.current = false
    setCampaignUnlocked(false)
    setCampaignConfirmed(false)
    setShowPlayerInfo(true)
  }, [debugFakeLogin, setShowPlayerInfo])

  async function handleResetPlayer() {
    setShowResetConfirm(true)
  }

  async function confirmResetPlayer() {
    try {
      await resetBeamSession()
    } catch {}
    try { window.sessionStorage?.removeItem('BEAM_TAB_INSTANCE_TAG') } catch {}
    try {
      const url = new URL(window.location.href)
      url.searchParams.set('beam_new', '1')
      window.location.href = url.toString()
      return
    } catch {}
    window.location.reload()
  }

  useEffect(() => {
    if (typeof window === "undefined") return
    const stored = window.localStorage.getItem(VOLUME_STORAGE_KEY)
    const parsed = stored !== null ? Number.parseFloat(stored) : Number.NaN
    const initial = Number.isFinite(parsed) ? Math.min(1, Math.max(0, parsed)) : getMasterVolume()
    setVolume(initial)
    setMasterVolume(initial)
  }, [])

  const handleVolumeChange = useCallback((next: number) => {
    const clamped = Math.min(1, Math.max(0, next))
    setVolume(clamped)
    setMasterVolume(clamped)
    if (typeof window !== "undefined") {
      try {
        window.localStorage.setItem(VOLUME_STORAGE_KEY, clamped.toString())
      } catch {
        // Ignore write failures
      }
    }
  }, [])

  const handleToggleAudioSettings = useCallback(() => {
    setShowAudioSettings((prev) => !prev)
  }, [])

  const handleCloseAudioSettings = useCallback(() => {
    setShowAudioSettings(false)
  }, [])

  const handleAdvanceCampaignStage = useCallback(() => {
    if (!nextStage || !canAdvanceStage) return
    selectStage(nextStage.id)
  }, [nextStage, canAdvanceStage, selectStage])

  const campaignSelectionProps = shouldShowCampaignOverlay
    ? {
        activeStage,
        stageProgress,
        selectedStageId: activeStage.id,
        pendingMechanics,
        campaignComplete,
        loopCount,
        onStartNextLoop: startNextLoop,
        onSelectStage: selectStage,
        onAcknowledgeMechanics: handleAcknowledgeMechanics,
        onConfirm: handleConfirmCampaignStage,
      }
    : undefined

  const loopAdvanceRef = useRef(false)
  useEffect(() => {
    if (gameState !== "won") {
      if (gameState === "playing") {
        campaignWinStageRef.current = null
        loopAdvanceRef.current = false
      }
      return
    }
    if (campaignWinStageRef.current === activeStage.id) {
      return
    }
    markStageComplete(activeStage.id)
    campaignWinStageRef.current = activeStage.id
    if (activeStage.order === totalStages - 1 && campaignComplete && !loopAdvanceRef.current) {
      loopAdvanceRef.current = true
      startNextLoop()
      setCampaignConfirmed(false)
      setShowPlayerInfo(true)
    }
  }, [gameState, activeStage.id, markStageComplete, activeStage.order, totalStages, campaignComplete, startNextLoop, setShowPlayerInfo])


  // ============================================================================
  // UI RENDERING

  return (
    <GameShell
      hudProps={{
        score,
        ballsLeft,
        remainingTowers,
        towerCount,
        stageLabel,
        stageName: activeStage.name,
        loopLabel,
        currencyAmount,
        alias,
        playerId,
        isCharging,
        powerSnapshot,
        selectedBallInfo: selectedBallInfo || undefined,
        onResetPlayer: handleResetPlayer,
        canShowRestart: hasShot && gameState === "playing",
        onRestart: resetGame,
        isAudioSettingsOpen: showAudioSettings,
        onToggleAudioSettings: handleToggleAudioSettings,
        onShowCommandDeck: () => {
          setCampaignConfirmed(false)
          setShowPlayerInfo(true)
        },
        showDebugControls: IS_DEV,
        onDebugSkipStage: IS_DEV ? handleDebugSkipStage : undefined,
        onDebugFakeLogin: IS_DEV ? handleDebugFakeLogin : undefined,
      }}
      campaignPanel={null}
      surfaceProps={{
        canvasRef,
        canvasWidth: CONST.CANVAS_WIDTH,
        canvasHeight: CONST.CANVAS_HEIGHT,
        readyForGame,
        handlePointerDown,
        handlePointerMove,
        handlePointerUp,
        overlayProps: {
          beamReady,
          readyForGame,
          hasShot,
          gameState,
          showPlayerInfo,
          alias,
          aliasModalOpen,
          aliasInput,
          aliasError,
          aliasSaving,
          aliasCanSave,
          playerId,
          stellarExternalId,
          stellarExternalIdentityId,
          pendingSignUrl,
          signatureError,
          walletPopupBlocked,
          walletPopupBlockedUrl,
          walletPopupContext,
          ballTypes: availableBallConfigs,
          selectedBallType,
          selectedBallInfo: selectedBallInfo || undefined,
          ballsLeft,
          score,
          coinsEarned,
          victoryBonusMultiplier: CONST.VICTORY_BONUS_MULTIPLIER,
          showResetConfirm,
          onCancelReset: () => setShowResetConfirm(false),
          onConfirmReset: confirmResetPlayer,
          onSelectBall: selectBallType,
          onStartFirstShot: startFirstShot,
          onAliasChange: handleAliasInputChange,
          onAliasSave: handleAliasSave,
          onAttachClick: handleAttachExternalId,
          onRetryAttach: handleRetryAttach,
          onResetPlayer: handleResetPlayer,
          onManualWalletOpen: handleManualWalletOpen,
          onClosePlayerInfo: () => setShowPlayerInfo(false),
          onRetry: resetGame,
          showAudioSettings,
          onCloseAudioSettings: handleCloseAudioSettings,
          volume,
          onVolumeChange: handleVolumeChange,
          showCampaignOverlay: shouldShowCampaignOverlay,
          campaignSelectionProps,
          campaignContext: {
            stageName: activeStage.name,
            stageLabel,
            stageType: activeStage.type,
            nextStageName: nextStage?.name,
            canAdvance: canAdvanceStage,
            onAdvance: handleAdvanceCampaignStage,
            campaignComplete,
            loopCount,
            onStartLoop: startNextLoop,
          },
        },
      }}
    />
  )
}
