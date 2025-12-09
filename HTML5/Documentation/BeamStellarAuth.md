# Beam & Stellar Auth Flow &#x1F513;

Focused guide on identity bootstrap, custodial wallet attach, and external Stellar wallet connect/sign.

## Flow Overview
- &#x1F464; **Alias entry**: Players set an alias (letters only) before gameplay. Saved to Beam stats via `saveAliasAndAttachWallet` in `lib/beam/player.ts`.
- &#x1F510; **Custodial attach**: `attachCustodialWallet` (via `beam.account.addExternalIdentity`) binds a custodial Stellar identity to the player after alias save.
- &#x1F6E1;&#xFE0F; **External wallet attach**: Uses `stellarFederationClient` + Beam account APIs to link an external Stellar wallet through a challenge/sign flow.
- &#x1F680; **Bootstrap**: `lib/beam.ts` resolves CID/PID/env from `.beamable` or env vars, initializes Beam with `clientServices`, registers `StellarFederationClient`, and auto-guests for token coverage.

## Client Entry Points
- `hooks/useBeamIdentity.ts`: Boots Beam, manages alias input/save, tracks custodial/external IDs.
- `hooks/useExternalIdentityFlow.ts`: Orchestrates wallet connect, challenge request, signature completion, popup handling.
- `hooks/useWalletBridge.ts`: Manages popup windows (prime, open, blocked-state recovery).
- `lib/beam/player.ts`: Core helpers for attach/login, challenge handling, WalletConnect URL builder.
- `beamable/clients/StellarFederationClient.ts`: Generated microservice client with `addItem`, `purchaseBall`, `updateCurrency`, `sendTestNotification`, `stellarConfiguration`, etc.

## External Wallet Connect (Challenge/Sign)
1) **Request connect URL**: `buildWalletConnectUrl(playerId)` builds the wallet URL from `stellarFederationClient.stellarConfiguration()` (network + bridge).
2) **Address subscription**: Subscribe to `external-auth-address` via `subscribeToExternalContext`; receive the wallet address payload.
3) **Challenge request**: `requestExternalIdentityChallenge(token)` (token = wallet address) triggers Beam external identity add; server returns `challenge_token`.
4) **Sign prompt**: `useExternalIdentityFlow` builds a sign URL (WalletConnect-style) embedding the challenge; user signs in the wallet popup.
5) **Signature subscription**: Listen to `external-auth-signature`; on signature, call `completeExternalIdentityChallenge(challengeToken, signature)` to finish attach and refresh profile.
6) **Login with external** (optional): `loginExternalIdentityToken` can authenticate via external identity token.

## Popup & UX Notes
- Popups can be blocked; `useWalletBridge` tracks blocked state and provides manual open via logged URL.
- `primeWalletWindow` pre-opens a window with a placeholder to increase allow rates.
- Blocked recovery retries a few times; users can click “open manually” if needed.

## Error Handling
- Signature conflicts (wallet already attached elsewhere) surface as user-friendly text in `formatSignatureErrorMessage` within `useExternalIdentityFlow`.
- Challenge/attach promises are guarded to avoid duplicate resolves (`createDeferred` in `lib/beam/player.ts`).
- Websocket parsing is defensive; malformed messages are skipped with warnings.
