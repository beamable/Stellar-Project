"use client"

import type React from "react"
import { useState, useEffect, useRef, useCallback } from "react"
import useBeamIdentity from "@/hooks/useBeamIdentity"
import useWalletBridge from "@/hooks/useWalletBridge"
import GameShell from "@/components/Game/GameShell"
import useAudioSettings from "@/hooks/useAudioSettings"
import useShop from "@/hooks/useShop"
import useExternalIdentityFlow from "@/hooks/useExternalIdentityFlow"
import useCampaignOverlay from "@/hooks/useCampaignOverlay"

const SHOULD_LOG_RENDERS = process.env.NEXT_PUBLIC_TD_RENDER_DEBUG === "true"
const IS_DEV = process.env.NODE_ENV !== "production"

function useRenderCounter(label: string, enabled: boolean) {
  const renderCountRef = useRef(0)
  useEffect(() => {
    renderCountRef.current += 1
    if (!enabled) return
    debugLog(`[RenderCounter] ${label} render #${renderCountRef.current}`)
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
} from "@/lib/beam/player"
import getBeam from "@/lib/beam"
import { debugLog } from "@/lib/debugLog"

export default function TowerDestroyer() {
  useRenderCounter("TowerDestroyer", SHOULD_LOG_RENDERS)
  const [showResetConfirm, setShowResetConfirm] = useState(false)
  const {
    volume,
    showAudioSettings,
    onVolumeChange,
    onToggleAudioSettings,
    onCloseAudioSettings,
  } = useAudioSettings()
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
  const refreshInventory = useCallback(() => {
    setInventoryRefreshKey((prev) => prev + 1)
  }, [])
  const { ballTypes, ballTypeMap, ownedBallTypes, loading: ballLoadoutLoading } = useBallLoadout(
    readyForGame,
    inventoryRefreshKey,
  )
  const { amount: currencyAmount, loading: currencyLoading } = useCurrency(readyForGame, inventoryRefreshKey)
  const inventoryInitialized = readyForGame && !ballLoadoutLoading && !currencyLoading
  const { showShop, openShop, closeShop, purchaseListing } = useShop({
    inventoryInitialized,
    onHidePlayerInfo: () => setShowPlayerInfo(false),
    onRefreshInventory: refreshInventory,
  })
  const {
    store: storeContent,
    listings: storeListings,
    loading: commerceLoading,
    error: commerceError,
  } = useCommerceManager({
    enabled: inventoryInitialized,
    refreshKey: inventoryRefreshKey,
  })
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
  const walletBridge = useWalletBridge()
  const campaignWinStageRef = useRef<string | null>(null)
  const {
    shouldShowCampaignOverlay,
    confirmCampaignStage,
    showCommandDeck,
    markCommandDeckSeen,
    setCampaignConfirmed,
    resetCampaignOverlay,
  } = useCampaignOverlay({
    readyForGame,
    activeStage,
    showPlayerInfo,
    setShowPlayerInfo,
  })
  const {
    pendingSignUrl,
    signatureError,
    walletPopupBlocked,
    walletPopupBlockedUrl,
    walletPopupContext,
    handleAttachExternalId,
    handleRetryAttach,
    handleManualWalletOpen,
  } = useExternalIdentityFlow({
    playerId,
    aliasModalOpen,
    refreshPlayerProfile,
    walletBridge: {
      pendingSignUrl: walletBridge.pendingSignUrl,
      setPendingSignUrl: walletBridge.setPendingSignUrl,
      signatureError: walletBridge.signatureError,
      setSignatureError: walletBridge.setSignatureError,
      blockedState: walletBridge.blockedState,
      acknowledgeUserAction: walletBridge.acknowledgeUserAction,
      clearBlockedState: walletBridge.clearBlockedState,
      openWalletWindow: (url, context, opts) => {
        walletBridge.openWalletWindow(url, context ?? "Stellar wallet", opts)
      },
      primeWalletWindow: walletBridge.primeWalletWindow,
      closeWalletWindow: walletBridge.closeWalletWindow,
      resetWalletBridge: walletBridge.reset,
    },
  })

  const openPlayerInfo = useCallback(() => {
    markCommandDeckSeen()
    refreshInventory()
    setShowPlayerInfo(true)
  }, [markCommandDeckSeen, refreshInventory, setShowPlayerInfo])

  const handleShowCommandDeck = useCallback(() => {
    showCommandDeck()
    markCommandDeckSeen()
    refreshInventory()
  }, [markCommandDeckSeen, refreshInventory, showCommandDeck])

  const handleAcknowledgeMechanics = useCallback(() => {
    if (pendingMechanics.length === 0) return
    acknowledgeMechanics(pendingMechanics)
  }, [acknowledgeMechanics, pendingMechanics])
  useEffect(() => {
    if (!inventoryInitialized) return
    if (commerceLoading) return
    if (!storeContent) return
    debugLog("[Commerce] Store resolved and ready:", {
      store: storeContent,
      listings: storeListings,
      error: commerceError,
    })
  }, [inventoryInitialized, commerceLoading, storeContent, storeListings, commerceError])

  useEffect(() => {
    if (!readyForGame) {
      coinsSyncedRef.current = false
      closeShop()
    }
  }, [readyForGame, closeShop])

  useEffect(() => {
    if (!readyForGame) {
      resetCampaignOverlay()
    }
  }, [readyForGame, resetCampaignOverlay])

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
          debugLog("[Coins] Syncing earned coins to server:", payload)
          await client.updateCurrency(payload)
          setInventoryRefreshKey((prev) => prev + 1)
        } else {
          console.warn("[Coins] StellarFederationClient.updateCurrency unavailable; skipping sync.")
        }
      } catch (err) {
        console.warn("[Coins] Failed to sync earned coins:", err)
      }
    })()
  }, [gameState, readyForGame, coinsEarned, setInventoryRefreshKey])
  const handleDebugSkipStage = useCallback(() => {
    if (!IS_DEV) return
    debugForceWin()
  }, [debugForceWin])

  const handleDebugFakeLogin = useCallback(() => {
    if (!IS_DEV) return
    debugFakeLogin()
    resetCampaignOverlay()
    openPlayerInfo()
  }, [debugFakeLogin, openPlayerInfo, resetCampaignOverlay])

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
        onConfirm: confirmCampaignStage,
        onOpenShop: openShop,
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
    // Only mark the stage that actually produced the win. If the user changes selection while still in a won state,
    // don't auto-complete the newly selected stage.
    if (campaignWinStageRef.current && campaignWinStageRef.current !== activeStage.id) {
      return
    }
    if (!campaignWinStageRef.current) {
      campaignWinStageRef.current = activeStage.id
      markStageComplete(activeStage.id)
    }
    if (activeStage.order === totalStages - 1 && campaignComplete && !loopAdvanceRef.current) {
      loopAdvanceRef.current = true
      Promise.resolve().then(() => {
        startNextLoop()
        setCampaignConfirmed(false)
        openPlayerInfo()
      })
    }
  }, [gameState, activeStage.id, markStageComplete, activeStage.order, totalStages, campaignComplete, startNextLoop, setCampaignConfirmed, openPlayerInfo])


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
        onToggleAudioSettings: onToggleAudioSettings,
        onShowCommandDeck: handleShowCommandDeck,
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
          onCloseAudioSettings,
          volume,
          onVolumeChange,
          onOpenShop: openShop,
          onCloseShop: closeShop,
          showShop,
          commerceLoading,
          commerceError,
          storeContent,
          storeListings: storeListings ?? [],
          currencyAmount,
          onRefreshCommerce: refreshInventory,
          ballTypeMap,
          ownedBallTypes,
          onPurchaseListing: purchaseListing,
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
