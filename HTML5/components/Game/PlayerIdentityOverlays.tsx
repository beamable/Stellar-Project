"use client"

import PlayerInfoOverlay from "@/components/Game/PlayerInfoOverlay"
import AliasSetupOverlay from "@/components/Game/AliasSetupOverlay"

type PlayerIdentityOverlaysProps = {
  beamReady: boolean
  readyForGame: boolean
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
  onAliasChange: (value: string) => void
  onAliasSave: () => void
  onAttachClick: () => void
  onRetryAttach: () => void
  onResetPlayer: () => void
  onManualWalletOpen: () => void
  onClosePlayerInfo: () => void
}

export default function PlayerIdentityOverlays({
  beamReady,
  readyForGame,
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
  onAliasChange,
  onAliasSave,
  onAttachClick,
  onRetryAttach,
  onResetPlayer,
  onManualWalletOpen,
  onClosePlayerInfo,
}: PlayerIdentityOverlaysProps) {
  return (
    <>
      {beamReady && readyForGame && showPlayerInfo && (
        <PlayerInfoOverlay
          playerId={playerId}
          alias={alias}
          stellarExternalId={stellarExternalId}
          stellarExternalIdentityId={stellarExternalIdentityId}
          pendingSignUrl={pendingSignUrl}
          signatureError={signatureError}
          walletPopupBlocked={walletPopupBlocked}
          walletPopupBlockedUrl={walletPopupBlockedUrl}
          walletPopupContext={walletPopupContext}
          onAttachClick={onAttachClick}
          onRetryAttach={onRetryAttach}
          onResetPlayer={onResetPlayer}
          onManualWalletOpen={onManualWalletOpen}
          onClose={onClosePlayerInfo}
        />
      )}
      {beamReady && (!alias || alias.length === 0 || aliasModalOpen) && (
        <AliasSetupOverlay
          aliasInput={aliasInput}
          aliasError={aliasError}
          aliasSaving={aliasSaving}
          canSave={aliasCanSave}
          onAliasChange={onAliasChange}
          onSaveAlias={onAliasSave}
        />
      )}
    </>
  )
}
