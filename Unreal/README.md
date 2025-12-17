# 🎮 Stellar Federation (Unreal)
Use this quest log to navigate the Unreal client and its Beamable + Stellar integration (inventory, shop, and external wallet flow).

## 🗺️ Map of the Realm
- 📂 Game code: `Source/Unreal2dDungeon` (runtime subsystems + Blueprint helpers).
- 🤖 Generated MS client: `Plugins/Unreal2dDungeonMicroserviceClients` (`UBeamStellarFederationApi` Blueprint/C++ wrappers for the StellarFederation microservice).
- 🛰️ Microservice code: `BeamableServices/services/StellarFederation` (+ `StellarFederationCommon`, `StellarFederationStorage`).
- 🌱 Content types: `Source/Unreal2dDungeon/Public/Content` (`UStellarCoin` currency, `UWarriorItem` NFT implementing `INftBase`).
- 🔔 Notifications & helpers: `UBeamOAuthNotifications` (custom notification subsystem) and `UStellarController` (IDs/channels + URL encoding for wallet flows).

## ▶️ Unreal Gameplay Loop (Beamable-Powered)
- 🔧 Boot: Beamable is wired in `Unreal2dDungeon.Target.cs`/`Build.cs`; runtime auto-initializes Beam contexts and generated microservice clients.
- 🪪 Identity: `UStellarController::GetStellarSettings` surfaces microservice ID, federation namespaces, and external auth channels; Blueprint/UI can render WalletConnect URLs and challenges.
- 🛍️ Shop: `PurchaseWarrior` (microservice) invokes Beam Commerce to buy a warrior listing; `DoOwnWarrior` checks ownership via inventory for gating UI/gameplay.
- 🪙 Inventory & Currency: `UpdateCurrency`, `AddItem`, `RemoveItem` endpoints mutate Beam inventory; `UBeamStellarFederationApi` exposes these to Blueprints/C++ for pickups, rewards, and starter grants.
- 🔔 External wallet attach: subscribe to `external-auth-address` / `external-auth-signature` via `UBeamOAuthNotifications` to drive the wallet-connect handshake and attach external identities.
- 🔄 Persistence: inventory/currencies live in Beam + StellarFederation; state survives sessions and syncs across devices.

## ⚔️ Warrior Dungeon Loop
- 🧭 Enter: player selects a `UWarriorItem` they own (`DoOwnWarrior` gate) and enters a dungeon run.
- 🗡️ Combat/Progress: gameplay drives warrior actions; loot/currency drops are tallied for the run.
- 🎁 Rewards: on clear or exit, grant coins via `UpdateCurrency` and warriors/loot via `AddItem` (with properties if needed).
- 🛠️ Persist: rewards are committed through the microservice so Beam inventory reflects the run outcome across sessions/devices.
- 🛍️ Reinvest: players spend coins in the shop (`PurchaseWarrior`) to expand their roster; new warriors then unlock deeper runs.

## 🧠 Microservice Core (StellarFederation)
- 📍 Location: `BeamableServices/services/StellarFederation` (Unreal version).
- 🚀 Init: config validation (`StellarRpc`), realm wallet bootstrap, Mongo extensions, and service registration (`ServiceRegistration`, `Endpoints`).
- 🪪 Identity: implements `IFederatedLogin`/`IFederatedInventory` for `StellarWeb3Identity` and `StellarWeb3ExternalIdentity`; `AuthenticateEndpoint` / `AuthenticateExternalEndpoint` manage custodial vs external login.
- 📡 Gameplay endpoints: `StellarConfiguration`, `UpdateCurrency`, `AddItem` (with duplicate guard), `RemoveItem`, `PurchaseWarrior` (Commerce API), `DoOwnWarrior` (ownership check). Inventory transaction endpoints are scaffolded for federation sync.
- 🧰 Clients: Generated into `Plugins/Unreal2dDungeonMicroserviceClients` (`UBeamStellarFederationApi`), giving Blueprint and C++ access with retry contexts and operation handles.
- [Microservice readme](BeamableServices/README.md)

## 🔐 Identity & Wallets
- 🔒 Custodial: Beam auth slots map to Stellar federation IDs; microservice keeps realm wallet and validates config before serving requests.
- 🌉 External: wallet-connect style flow uses the channels from `UStellarController`; `UBeamOAuthNotifications` subscribes and broadcasts `FOAuthNotificationMessage` to Blueprint listeners for address/signature capture.
- ✅ Verification: external auth endpoints validate signatures; custodial flow simply requires existing vault entries.

## 💰 Content, Contracts, Inventory
- 🪙 Currency: `UStellarCoin` (Beam currency content) represents the on-chain coin; `UpdateCurrency` mints/credits via Beam inventory.
- 🛡️ Items: `UWarriorItem` implements `INftBase` to expose name/description/image/custom properties; microservice `AddItem` guards against duplicates.
- 🏪 Commerce: `PurchaseWarrior` bridges Beam Commerce API to deliver warriors; pair with Blueprint UI to present listings and call the generated client.
- 🧾 State access: `DoOwnWarrior` queries current items (via Beam Inventory Service) to drive UI locks and gameplay progression.

## 🔁 Key Flows
- 🧑‍🌾 Onboard: login/guest -> fetch Stellar config -> attach custodial identity -> inventory seeded (coin + starter warrior) via `AddItem/UpdateCurrency`.
- 🛍️ Purchase: player buys a warrior listing -> microservice calls Commerce -> Beam inventory updates -> `DoOwnWarrior` confirms ownership for gameplay unlocks.
- 🌉 External attach: wallet-connect UI -> address/signature notifications -> external identity attach -> federated inventory available for that wallet namespace.


