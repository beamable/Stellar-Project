# Backend Parity &#x1F3E0;

Where the backend lives and how it aligns with the generated Stellar federation client.

## Service Location
- `BeamableServices/services/StellarFederation`: C# microservice project (Dockerfile, endpoints, federation IDs, and service registration).
- Key files: `StellarFederation.cs`, `StellarFederationExternal.cs`, `ServiceRegistration.cs`, `federations.json`.

## Generated Client
- `beamable/clients/StellarFederationClient.ts`: Auto-generated microservice client consumed by `lib/beam.ts` (`beam.use(StellarFederationClient)`).
- Exposes endpoints: `stellarConfiguration`, `addItem`, `addUniqueItem`, `removeItem`, `purchaseBall`, `updateCurrency`, `getRealmAccount`, `generateRealmAccount`, `externalAddress`, `externalSignature`.
- Types in `beamable/clients/types/index.ts` mirror the microservice contract (including the external wallet callback payloads).

## Usage in Frontend
- Registered in `lib/beam.ts` after Beam init (`clientServices(beam)`), then accessed via `beam.stellarFederationClient`.
- Called from:
  - `lib/beam/player.ts`: wallet connect URLs, external identity handling, wallet callback wiring.
  - `hooks/useBallLoadout.ts`: default ball grant via `addItem`.
  - `hooks/useCoinSync.ts`: coin sync via `updateCurrency` (defaults to `currency.coin.beam_coin`).
  - `hooks/useShop.ts`: purchases via `purchaseBall`.

## Keeping Parity
- Regenerate the client whenever backend endpoints change.
- Ensure `federations.json` IDs match `StellarFederationClient.federationIds`.
- After backend changes, re-run `npm run test` and verify wallet attach, content grants, and commerce flows end-to-end.
