## Beam SDK Integration Notes

This project uses `beamable-sdk@1.0.x`. The client bootstrap flow lives in `lib/beam.ts` and follows the v1 patterns:

- **Environment resolution** – `resolveBeamConfig()` now understands `.beamable/connection-configuration.json`, `/api/beam-config`, and `window.__BEAM__`. When a host is provided we register a custom environment via `BeamEnvironment.register` so `Beam.init` receives a proper `environment` key.
- **Service registration** – After `Beam.init` completes we invoke `clientServices(beam)` to wire up all first-party services (account/auth/stats/etc.) and then register our generated `StellarFederationClient`. Any new Beam services become available automatically.
- **Realtime notifications** – Custom notification helpers still live in `lib/notifications.ts`. See the “Realtime Notifications” section below for the current behavior and caveats.

### Configuration Tips

1. Keep `.beamable/connection-configuration.json` in sync (CID, PID, and optional `host`). The `/api/beam-config` route surfaces these values to the client.
2. Set `NEXT_PUBLIC_BEAM_ENV` / `BEAM_ENV` to `prod`, `stg`, `dev`, or the custom environment key if you register your own cluster.
3. To override the API host without editing `.beamable`, set `NEXT_PUBLIC_BEAM_HOST` or `BEAM_HOST` (e.g., for QA stacks). The resolver creates a temporary environment for you.

Whenever you update SDK versions, re-import the `clientServices` helper and re-run `npm run test` to ensure our identity/notifications flows still work end-to-end.

### Realtime Notifications

We still consume Beam notifications through the legacy websocket shim to support the external identity flow. Highlights:

- Subscriptions live in `lib/notifications.ts`. They call `getBeam()` (so Beam must be initialized first), grab `beam.ws.rawSocket`, and install a `message` listener that filters by `payload.context`.
- We normalize payloads by unwrapping nested `payload` / `messageFull` fields and force the `context` casing so downstream handlers receive a consistent shape.
- `subscribeExternalAuthNotifications` wires up both `external-auth-address` and `external-auth-signature` contexts, returning a stopper that detaches the listeners.

This approach predates the v1.0 `beam.on/beam.off` helpers because our custom payloads (and Stellar wallet handshake) rely on the raw message stream. When we revisit the external auth UX we can migrate to the official subscription API, but for now this doc reflects the production implementation.
