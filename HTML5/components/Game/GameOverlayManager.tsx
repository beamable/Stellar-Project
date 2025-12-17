"use client"

import type { BallType, BallTypeConfig } from "@/components/Game/types"
import BallSelectionOverlay from "@/components/Game/BallSelectionOverlay"
import PlayerIdentityOverlays from "@/components/Game/PlayerIdentityOverlays"
import BeamInitializingOverlay from "@/components/Game/BeamInitializingOverlay"
import ResetConfirmOverlay from "@/components/Game/ResetConfirmOverlay"
import ResultOverlay, { type CampaignResultContext } from "@/components/Game/ResultOverlay"
import AudioSettingsOverlay from "@/components/Game/AudioSettingsOverlay"
import CampaignSelectionOverlay from "@/components/Game/CampaignSelectionOverlay"
import ShopOverlay from "@/components/Game/ShopOverlay"
import type { CampaignStage, MechanicTag } from "@/components/Game/campaign"
import type { StageProgress } from "@/hooks/useCampaignProgress"
import type { ListingContent, StoreContent } from "beamable-sdk"

type GameOverlayManagerProps = {
  beamReady: boolean
  readyForGame: boolean
  hasShot: boolean
  gameState: "playing" | "won" | "gameOver"
  showPlayerInfo: boolean
  alias: string | null
  aliasModalOpen: boolean
  aliasInput: string
  aliasError: string | null
  aliasSaving: boolean
  aliasCanSave: boolean
  playerId: string | null
  stellarExternalId: string | null
  stellarExternalIdentityId: string | null
  pendingSignUrl: string | null
  signatureError: string | null
  walletPopupBlocked: boolean
  walletPopupBlockedUrl: string | null
  walletPopupContext: string | null
  ballTypes: BallTypeConfig[]
  selectedBallType: BallType
  selectedBallInfo?: BallTypeConfig
  ballsLeft: number
  score: number
  coinsEarned: number
  victoryBonusMultiplier: number
  showResetConfirm: boolean
  onCancelReset: () => void
  onConfirmReset: () => void
  onSelectBall: (type: BallType) => void
  onStartFirstShot: () => void
  onAliasChange: (value: string) => void
  onAliasSave: () => void
  onAttachClick: () => void
  onRetryAttach: () => void
  onResetPlayer: () => void
  onManualWalletOpen: () => void
  onClosePlayerInfo: () => void
  onRetry: () => void
  showAudioSettings: boolean
  onCloseAudioSettings: () => void
  volume: number
  onVolumeChange: (volume: number) => void
  onOpenShop: () => void
  onCloseShop: () => void
  showShop: boolean
  commerceLoading: boolean
  commerceError: string | null
  storeContent: StoreContent | null
  storeListings: ListingContent[]
  currencyAmount: number | null
  onRefreshCommerce?: () => void
  ballTypeMap: Record<BallType, BallTypeConfig>
  ownedBallTypes: BallType[]
  onPurchaseListing: (listingId: string) => Promise<void>
  campaignContext?: CampaignResultContext
  showCampaignOverlay: boolean
  campaignSelectionProps?: {
    activeStage: CampaignStage
    stageProgress: StageProgress[]
    selectedStageId: string
    pendingMechanics: MechanicTag[]
    campaignComplete: boolean
    loopCount: number
    onStartNextLoop: () => void
    onSelectStage: (stageId: string) => void
    onAcknowledgeMechanics: () => void
    onConfirm: () => void
    onOpenShop: () => void
  }
}

export default function GameOverlayManager({
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
  ballTypes,
  selectedBallType,
  selectedBallInfo,
  ballsLeft,
  score,
  coinsEarned,
  victoryBonusMultiplier,
  showResetConfirm,
  onCancelReset,
  onConfirmReset,
  onSelectBall,
  onStartFirstShot,
  onAliasChange,
  onAliasSave,
  onAttachClick,
  onRetryAttach,
  onResetPlayer,
  onManualWalletOpen,
  onClosePlayerInfo,
  onRetry,
  showAudioSettings,
  onCloseAudioSettings,
  volume,
  onVolumeChange,
  onOpenShop,
  onCloseShop,
  showShop,
  commerceLoading,
  commerceError,
  storeContent,
  storeListings,
  currencyAmount,
  onRefreshCommerce,
  ballTypeMap,
  ownedBallTypes,
  onPurchaseListing,
  campaignContext,
  showCampaignOverlay,
  campaignSelectionProps,
}: GameOverlayManagerProps) {
  return (
    <>
      {!beamReady && <BeamInitializingOverlay />}

      {showCampaignOverlay && campaignSelectionProps && (
        <CampaignSelectionOverlay {...campaignSelectionProps} onOpenShop={onOpenShop} />
      )}

      {!hasShot && gameState === "playing" && readyForGame && !showPlayerInfo && !showCampaignOverlay && (
        <BallSelectionOverlay
          ballTypes={ballTypes}
          selectedBallType={selectedBallType}
          selectedBallInfo={selectedBallInfo}
          onSelectBall={onSelectBall}
          onStart={onStartFirstShot}
        />
      )}

      <PlayerIdentityOverlays
        beamReady={beamReady}
        readyForGame={readyForGame}
        showPlayerInfo={showPlayerInfo}
        alias={alias}
        aliasModalOpen={aliasModalOpen}
        aliasInput={aliasInput}
        aliasError={aliasError}
        aliasSaving={aliasSaving}
        aliasCanSave={aliasCanSave}
        playerId={playerId}
        stellarExternalId={stellarExternalId}
        stellarExternalIdentityId={stellarExternalIdentityId}
        pendingSignUrl={pendingSignUrl}
        signatureError={signatureError}
        walletPopupBlocked={walletPopupBlocked}
        walletPopupBlockedUrl={walletPopupBlockedUrl}
        walletPopupContext={walletPopupContext}
        onAliasChange={onAliasChange}
        onAliasSave={onAliasSave}
        onAttachClick={onAttachClick}
        onRetryAttach={onRetryAttach}
        onResetPlayer={onResetPlayer}
        onManualWalletOpen={onManualWalletOpen}
        onClosePlayerInfo={onClosePlayerInfo}
        onOpenShop={onOpenShop}
      />

      {showResetConfirm && <ResetConfirmOverlay onCancel={onCancelReset} onConfirm={onConfirmReset} />}

      <ResultOverlay
        gameState={gameState}
        score={score}
        coinsEarned={coinsEarned}
        ballsLeft={ballsLeft}
        victoryBonusMultiplier={victoryBonusMultiplier}
        onRetry={onRetry}
        campaignContext={campaignContext}
      />

      {showAudioSettings && (
        <AudioSettingsOverlay
          volume={volume}
          onVolumeChange={onVolumeChange}
          onClose={onCloseAudioSettings}
        />
      )}

      {showShop && (
        <ShopOverlay
          store={storeContent}
          listings={storeListings}
          loading={commerceLoading}
          error={commerceError}
          currencyAmount={currencyAmount}
          ballTypeMap={ballTypeMap}
          ownedBallTypes={ownedBallTypes}
          onRefresh={onRefreshCommerce}
          onPurchase={onPurchaseListing}
          onClose={onCloseShop}
        />
      )}
    </>
  )
}
