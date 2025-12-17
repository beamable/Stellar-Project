# Stellar HTML5 Demo (Beamable + Stellar) &#x1F3AE;

This repository showcases how to integrate the Stellar SDK within the Beamable ecosystem for an HTML5 game built on Next.js. It covers Beam bootstrap, custodial and external wallet attachment, content/inventory sync, commerce, and the Tower Destroyer gameplay loop.

## What to Expect In-Game
- &#x1F3AF; Arcade slingshot loop: fire balls to topple towers, earn score, and clear stages.
- &#x1F5FA;&#xFE0F; Progression: advance through campaign stages, repeat loops for higher difficulty, and unlock mechanics as you go.
- &#x1F9F0; Loadouts: collect different ball types (normal, multishot, fire, laser) with varying speed/power; a default ball is granted automatically.
- &#x1F4B0; Economy: earn coins from wins, sync them to Beam/Stellar, and purchase ball listings from the in-game shop.
- &#x23F3; **Minting delay**: Stellar mints settle on a fixed cadence; after purchases/grants/coin sync it can take ~10–15 seconds before inventory reflects the change. The UI keeps the loading/refresh state up during this window; if something looks “stuck”, wait a moment and then tap **Refresh** in the shop.
- &#x1F6E1;&#xFE0F; Identity flow: set an alias, auto-attach a custodial wallet, and optionally attach an external Stellar wallet via WalletConnect-style popups.

## Quick Start
- &#x1F680; `npm install`
- &#x1F5C2;&#xFE0F; Add `.beamable/connection-configuration.json` with your CID/PID (see below).
- &#x1F9EA; Create `.env.local` as needed and run `npm run dev`.

## Configuration
Primary source of truth: `.beamable/connection-configuration.json` (consumed via `app/api/beam-config/route.ts`):
```json
{ "cid": "your-cid", "pid": "your-pid", "host": "https://api.your-cluster.com" }
```
Environment overrides:
- `NEXT_PUBLIC_BEAM_ENV` / `BEAM_ENV`: environment key (`prod`, `stg`, `dev`, or custom).
- `NEXT_PUBLIC_BEAM_HOST` / `BEAM_HOST`: override API host without editing `.beamable`.
- `NEXT_PUBLIC_BEAM_CID` / `NEXT_PUBLIC_BEAM_PID`: fallback if the file is missing.
- `NEXT_PUBLIC_STORE_CONTENT_ID`: store content id for commerce (default `stores.Store_Nf`).
- `NEXT_PUBLIC_DEBUG_LOGS=true`: enable verbose client logging.

Example `.env.local`:
```
NEXT_PUBLIC_BEAM_ENV=stg
NEXT_PUBLIC_BEAM_HOST=https://api.stg.beamable.com
NEXT_PUBLIC_STORE_CONTENT_ID=stores.Store_Nf
NEXT_PUBLIC_DEBUG_LOGS=true
```

## NPM Scripts
- &#x1F6E0;&#xFE0F; `npm run dev`: Start Next.js in development.
- &#x1F4E6; `npm run build`: Production build.
- &#x1F6A6; `npm run start`: Serve the built app.
- &#x1F50D; `npm run lint`: Lint via `scripts/run-lint.cjs`.
- &#x2705; `npm run test`: Vitest suites (physics, content caching, commerce caching).

## Architecture Overview
- &#x2728; **Beam bootstrap (`lib/beam.ts`)**: Resolves config from `.beamable`, env vars, or `window.__BEAM__`; registers custom environments when a host override is present; initializes Beam with `clientServices` and the generated `StellarFederationClient`; auto-logs guests to ensure tokens.
- &#x2B50; **Stellar federation (`lib/beam/player.ts`, `beamable/clients/StellarFederationClient.ts`)**: Custodial identity attach, external identity attach/login, WalletConnect URL builder, and microservice endpoints (`addItem`, `purchaseBall`, `updateCurrency`, plus the external wallet callback endpoints `externalAddress` / `externalSignature`).
- &#x1F514; **Notifications (`lib/notifications.ts`)**: Raw websocket subscriptions for `external-auth-address` and `external-auth-signature`, with payload normalization for the wallet handshake.
- &#x1F4E6; **Content / Inventory / Commerce (`lib/beamContent.ts`, `lib/beamInventory.ts`, `lib/commerceManager.ts`)**: Content fetch and caching, inventory helpers over the generated OpenAPI calls, store + listing resolution with cache keys by store/manifest.
- &#x1F3D7;&#xFE0F; **Game loop and UX (`components/Game/*`, `hooks/*`)**: Canvas physics and state, campaign progression, ball loadouts, currency sync, shop purchases, identity bootstrap, wallet pop-up orchestration, and overlays for player info and campaign flow.

## Workflow Tips
- &#x1F4CC; Keep `.beamable/connection-configuration.json` aligned with your target cluster; prefer env overrides for quick host/env switches.
- &#x1FA9F; If a popup blocker prevents wallet attach, use the logged URL or enable popups and retry.
- &#x2705; After SDK updates, re-run `npm run test` and sanity-check wallet attach and commerce flows end-to-end.
