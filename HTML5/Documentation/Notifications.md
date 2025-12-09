# Notifications & Websocket Strategy &#x1F514;

How notification contexts are consumed and why raw websocket access is used.

## Contexts
- `external-auth-address`: Wallet address broadcast during external attach.
- `external-auth-signature`: Wallet signature payload to finalize external attach.

## Rationale for Raw Websocket
- The external identity flow relies on custom payload shapes and `messageFull` fields.
- `lib/notifications.ts` taps `beam.ws.rawSocket` to parse and normalize messages (unwrap payload/messageFull, normalize `context` casing).
- This predates the v1 `beam.on/off` helpers; raw access ensures the custom Stellar handshake stays compatible.

## Subscription Helpers
- `subscribeToContext(context, handler)`: Installs a message listener on `beam.ws.rawSocket`, normalizes payloads, and filters by context (case-insensitive). Returns a stopper.
- `subscribeExternalAuthNotifications`: Convenience that wires both `external-auth-address` and `external-auth-signature`.
- `getNotificationBootstrap`: Calls `/basic/notification/` (with auth) to fetch bootstrap data (e.g., channel prefix).

## Usage in Wallet Flow
- `hooks/useExternalIdentityFlow.ts`:
  - Subscribes to `external-auth-address` to start the challenge request.
  - Subscribes to `external-auth-signature` to complete the challenge with the signed value.
  - Stops subscriptions after each phase to avoid duplicates.
- `lib/beam/player.ts`:
  - Exposes `subscribeToExternalContext` for reuse.
  - Parses bootstrap data to derive `cid/pid` when needed.

## Error Handling & Safety
- Defensive JSON parsing; malformed messages are ignored with warnings.
- Handlers wrapped in try/catch to avoid crashing subscriber callbacks.
- Stoppers detach listeners on cleanup; `useExternalIdentityFlow` also closes wallet windows on unmount/reset.
