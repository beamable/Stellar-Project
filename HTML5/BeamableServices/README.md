# Stellar Federation Microservice for Beamable

A production-ready Beamable microservice that implements federated authentication and inventory management using the Stellar blockchain. This service enables games to leverage Stellar smart contracts (Soroban) for managing player wallets, in-game currencies, and items as on-chain assets.

## Overview

This microservice bridges Beamable's game backend with Stellar's blockchain, providing:

- **Federated Authentication**: Wallet-based player authentication via Stellar accounts
- **Blockchain Inventory**: On-chain asset management for currencies and items
- **Smart Contract Deployment**: Automated deployment and compilation of Soroban contracts
- **Concurrent Transaction Processing**: Batched, asynchronous blockchain operations
- **Dual Wallet Support**: Both custodial and self-custody wallet options

## Beamable Federation Interfaces

This service implements two core Beamable federation interfaces:

### IFederatedLogin

Provides custom authentication federation, enabling players to authenticate using Stellar wallet signatures. Supports challenge-response flows for cryptographic wallet verification.

[Learn more about IFederatedLogin](https://github.com/beamable/FederatedAuthentication/blob/main/README.md)

### IFederatedInventory

Extends Beamable's inventory system with externally-managed items and currencies stored on the Stellar blockchain. **Note:** Implementing `IFederatedInventory<T>` automatically requires implementing `IFederatedLogin<T>` since federated inventory depends on federated authentication.

[Learn more about IFederatedInventory](https://github.com/beamable/FederatedInventory/blob/main/README.md)

## Wallet Identity Types

The service provides two implementations to support different custody models:

### StellarWeb3Identity (Custodial)

- **Use Case**: Traditional gaming experience where the service manages player wallets
- **Custody**: Service controls private keys (encrypted in MongoDB)
- **UX**: Seamless authentication, no external wallet required
- **Security**: Keys encrypted at rest, managed by the service

### StellarWeb3ExternalIdentity (Self-Custody)

- **Use Case**: Web3-native players who want full control of their assets
- **Custody**: Players control private keys via external wallets (Freighter, etc.)
- **UX**: Requires wallet connection and transaction signing
- **Security**: Non-custodial, players retain full ownership

Both implementations expose the same federated inventory interface, allowing games to support multiple wallet types simultaneously.

## Architecture

### Project Structure

```
BeamableServices/
├── services/
│   ├── StellarFederation/          # Main microservice (.NET 8.0)
│   ├── StellarFederationCommon/     # Shared types (netstandard2.1)
│   └── StellarFederationStorage/    # MongoDB storage models (.NET 8.0)
```

### Core Components

#### Contract Management
- **Rust Template System**: Soroban smart contracts defined as parameterized Rust templates
- **Automated Compilation**: Contracts compiled using embedded Stellar CLI
- **Dynamic Deployment**: Contracts deployed on-demand and addresses cached in MongoDB
- **Support for**: Fungible tokens, NFTs, and custom game logic

#### Transaction Processing
The service uses a sophisticated batch processing system:

1. **Queue System**: Inventory operations are queued to MongoDB with work group assignments
2. **Distributed Locking**: Each work group is protected by distributed locks to prevent conflicts
3. **Batch Optimization**: Multiple operations combined into single Stellar transactions
4. **Concurrent Execution**: Background service processes multiple work groups in parallel
5. **Retry Logic**: Exponential backoff for failed transactions

**Example Flow:**
```
Player Action → Queue to MongoDB → Background Service Picks Up →
Acquire Lock → Batch Operations → Submit to Stellar → Update Status
```

#### Blockchain Monitoring
- **Event Streaming**: Monitors Stellar Horizon API for transaction events
- **Soroban Integration**: Fetches smart contract invocation logs
- **Beamable Scheduler**: Uses Beamable's scheduling service for periodic chain log fetching
- **State Reconciliation**: Ensures local inventory state matches blockchain state

#### Logging & Observability
- **Operation Logging**: All contract calls and transactions logged to MongoDB
- **Status Tracking**: Transaction lifecycle tracked through states (pending → processing → completed/failed)
- **Audit Trail**: Complete history of blockchain operations for each player
- **Error Handling**: Detailed exception logging with custom exception types

### Background Service

A dedicated hosted service (`StellarBackgroundService`) runs concurrently with the main microservice:

- Polls MongoDB for queued transactions at configurable intervals
- Processes multiple work groups in parallel using distributed locking
- Batches operations efficiently (different limits for native XLM vs contract calls)
- Handles transaction failures with retry logic and error reporting
- Updates transaction status in real-time

### Storage

MongoDB collections store all persistent data:

| Collection | Purpose |
|------------|---------|
| `VaultCollection` | Encrypted private keys for custodial wallets |
| `ContractCollection` | Deployed contract addresses and metadata |
| `InventoryTransactionCollection` | Pending and completed inventory operations |
| `LockCollection` | Distributed locks for work group coordination |
| `ExternalAuthCollection` | External wallet authentication mappings |

## Key Features

### Smart Contract Templates

Pre-built Soroban contract templates for common game use cases:

- **coin.rs**: Fungible token implementation for in-game currencies, [readme](services/StellarFederation/Features/Contract/Templates/coin.rs.README.md)
- **gold.rs**: Example currency with custom transfer logic, [readme](services/StellarFederation/Features/Contract/Templates/gold.rs.README.md)
- **nft.rs**: Non-fungible token implementation for unique items, [readme](services/StellarFederation/Features/Contract/Templates/nft.rs.README.md)

Templates use Handlebars syntax for parameterization (token name, symbol, admin account, etc.).

### Configuration

Managed through Beamable's realm config system (namespace: `stellar-federation`):

| Setting | Description                                |
|---------|--------------------------------------------|
| `StellarRpc` | Soroban RPC endpoint URL                   |
| `StellarHorizon` | Horizon API endpoint URL                   |
| `StellarNetwork` | Network passphrase ("testnet" or "public") |
| `MessageQueueBatchLimit` | Max transactions per batch                 |
| `MessageQueueBatchLimit` | Max transactions per batch                 |
| `ExtraResourceFeePercentage` | Extra percentage for gas fee               |
| `FetchLogsCronSeconds` | Fetch chain logs every X seconds           |
| `WalletConnectBridgeUrl` | Url for the external wallet bridge app     |

All configuration valuse are updatable trough project [Portal](https://portal.beamable.com/login/) Configuration page and automatically applied.

### Endpoints

Custom REST endpoints for client integration:

- `AuthenticateEndpoint` - Authenticate player with wallet signature
- `AuthenticateExternalEndpoint` - Link external wallet to Beamable account
- `GetInventoryStateEndpoint` - Fetch current blockchain inventory state
- `StartInventoryTransactionEndpoint` - Initiate inventory modification

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Beamable CLI (`dotnet tool install --global beamable`)
- Stellar account for testnet/mainnet access
- MongoDB instance (provided by Beamable)
- **Wallet Integration Bridge** (required for `StellarWeb3ExternalIdentity`): A web application that enables players to connect self-custody wallets. Must be deployed to an HTTPS domain and configured via the `WalletConnectBridgeUrl` setting. See the [Wallet Integration README](../../Wallet%20Integration/Readme.md) for deployment instructions

### Content Items: Blockchain Asset Blueprints

Content items in this system serve as **blueprints for creating Stellar smart contracts** and defining the blockchain assets that will be deployed and managed on-chain. When you create a content item, you're defining the template for a fungible token (FT) or non-fungible token (NFT) that will be deployed as a Soroban smart contract. Instances of these content items become actual blockchain assets in player inventories.  

#### Content Item Types

The microservice recognizes two base interfaces for blockchain-backed content:

##### INftBase (Non-Fungible Tokens)

Content items implementing `INftBase` represent **NFTs** on the Stellar blockchain. Each instance is unique and has individual metadata.

**Required Properties:**
- `Name` - Token display name
- `Description` - Token description
- `Image` - Image URL or IPFS hash
- `CustomProperties` - Key-value pairs for custom metadata (e.g., stats, attributes)

**Example Implementation:**

```csharp
[ContentType("crop")]
public class CropItem : ItemContent, INftBase
{
    public CropItem()
    {
        // Configure federation to use Stellar microservice
        federation = new OptionalFederation
        {
            HasValue = true,
            Value = new Federation
            {
                Service = "StellarFederation",
                Namespace = "stellar"
            }
        };
    }

    [SerializeField] private string name = "";
    [SerializeField] private string description = "";
    [SerializeField] private string image = "";
    [SerializeField] private SerializableDictionaryStringToString customProperties = new();

    public string Name => name;
    public string Description => description;
    public string Image => image;
    public SerializableDictionaryStringToString CustomProperties => customProperties;
}
```

##### IFtBase (Fungible Tokens)

Content items implementing `IFtBase` represent **fungible tokens** (currencies) on the Stellar blockchain. All units are identical and interchangeable.

**Required Properties:**
- `Name` - Currency display name
- `Symbol` - Trading symbol (e.g., "GOLD")
- `Decimals` - Number of decimal places (0-7 for Stellar)
- `Image` - Currency icon URL
- `Description` - Currency description

**Example Implementation:**

```csharp
[ContentType("coin")]
public class CoinCurrency : CurrencyContent, IFtBase
{
    public CoinCurrency()
    {
        // Configure federation to use Stellar microservice
        federation = new OptionalFederation
        {
            HasValue = true,
            Value = new Federation
            {
                Service = "StellarFederation",
                Namespace = "stellar"
            }
        };
    }

    [SerializeField] private string _name = "";
    [SerializeField] private string _symbol = "";
    [SerializeField] private int _decimals = 0;
    [SerializeField] private string _image = "";
    [SerializeField] private string _description = "";

    public string Name => _name;
    public string Symbol => _symbol;
    public int Decimals => _decimals;
    public string Image => _image;
    public string Description => _description;
}
```

#### Creating Custom Content Types

Follow these steps to create new blockchain-backed content:

**1. Define the Content Class**

Create a new class in `StellarFederationCommon/FederationContent/`:

```csharp
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using StellarFederationCommon.Extensions;

namespace StellarFederationCommon.FederationContent
{
    [ContentType("weapon")] // Unique identifier
    public class WeaponItem : ItemContent, INftBase
    {
        // Constructor sets federation configuration
        public WeaponItem()
        {
            federation = new OptionalFederation
            {
                HasValue = true,
                Value = new Federation
                {
                    Service = StellarFederationSettings.MicroserviceName,
                    Namespace = StellarFederationSettings.StellarIdentityName
                }
            };
        }

        // Required INftBase properties
        [SerializeField] private string name = "";
        [SerializeField] private string description = "";
        [SerializeField] private string image = "";
        [SerializeField] private SerializableDictionaryStringToString customProperties = new();

        // Custom properties specific to weapons
        [SerializeField] private int damage = 0;
        [SerializeField] private string rarity = "common";

        // Implement interface
        public string Name => name;
        public string Description => description;
        public string Image => image;
        public SerializableDictionaryStringToString CustomProperties => customProperties;

        // Custom getters
        public int Damage => damage;
        public string Rarity => rarity;
    }
}
```

**2. Create Content Instances**

Content instances are **JSON files** that represent published Beamable content entries.

- **Location (HTML5 projects)**: Create them under `.beamable/content/<PID>/global/` where `<PID>` is your Beamable project id (for example, `.beamable/content/DE_1923097525143560/global/`).
- **Naming conventions (prefixes)**:
  - Currencies must start with `currency` (example: `currency.coin.beam_coin`)
  - Items must start with `items` (example: `items.ball.FireBall`)
  - Listings must start with `listings` (example: `listings.Listing_Fire`)
  - Stores must start with `stores` (example: `stores.Store_Nf`)

**CLI workflow (sync local JSONs to your realm)**
- `dotnet beam init`: Authenticate and bind the local project to your target CID/PID/host.
- `dotnet beam content status`: Verify what your realm currently has published (and confirm you're targeting the right `global` manifest).
- `dotnet beam content publish`: Publish your local `.beamable/content/<PID>/global/*` JSONs to the realm so the project uses the updated remote content.

**Getting the manifest id (`referenceManifestId`) when creating new content JSONs**
- `referenceManifestId` must match the **uid of the currently-published `global` manifest** in your realm.
- The easiest way to get it is to run `dotnet beam content publish` once; the publish response includes a manifest object like `{\"id\":\"global\",\"uid\":\"<MANIFEST_UID>\",...}`. Use that `uid` value in your content JSONs.
- If you're copying content JSONs from Unity, the `referenceManifestId` is frequently wrong for your HTML5 realm. Re-publish and update the JSONs to use your realm's current manifest `uid`.

Beamable currently doesn't provide a public documentation site listing “default” JSON shapes for each built-in content type. For now, the easiest starting point is to take the content JSONs in this repo (under the HTML5 project's `.beamable/content/...`) and adjust them to your liking.

**Alternative authoring workflow (recommended if you're already using Unity):**
- Create a Unity project and add the Beamable Unity SDK.
- Add content classes that match the exact properties of the content types you defined in `StellarFederationCommon/FederationContent/`.
- Use the Unity Beamable **Content Manager** to create/edit the content entries.
- Copy the generated `.beamable` folder (or just the `.beamable/content/<PID>/global/` subtree) from the Unity project into your HTML5 project.

**3. Smart Contract Deployment**

When a player first acquires an item of your custom content type:

1. The microservice detects it's a new content type
2. Uses the content properties to populate a Rust contract template
3. Compiles the Soroban contract using the embedded Stellar CLI
4. Deploys the contract to the Stellar blockchain
5. Stores the contract address in MongoDB for future use

#### Federation Configuration

The `federation` property is **critical** - it tells Beamable which microservice handles this content:

```csharp
federation = new OptionalFederation
{
    HasValue = true,
    Value = new Federation
    {
        Service = "StellarFederation",  // Microservice name
        Namespace = "stellar"            // Identity namespace
    }
};
```

Without this configuration, content won't be recognized as blockchain-backed and will be stored only in Beamable's database.

#### Content Validation

Add validation attributes to enforce constraints:

```csharp
using Beamable.Common.Content.Validation;

[SerializeField]
[CannotBeBlank]
[MustBePositive]
private int damage = 0;
```

Custom validators can be created by implementing `IValidationRule`.

#### Content References

Use `ContentRef<T>` or `ContentLink<T>` to reference content in code:

```csharp
[Serializable]
public class WeaponItemRef : ContentRef<WeaponItem> { }

[Serializable]
public class WeaponItemLink : ContentLink<WeaponItem> { }
```

Resolve at runtime:

```csharp
var weaponRef = new WeaponItemRef { Id = "items.weapon.sword" };
var weapon = await weaponRef.Resolve();
Console.WriteLine($"Weapon: {weapon.Name}, Damage: {weapon.Damage}");
```

#### Content Lifecycle

1. **Development**: Create/edit content
2. **Publishing**: Content published to Beamable backend as JSON
3. **First Use**: Smart contract deployed to Stellar when first player receives item
4. **Runtime**: Instances tracked on-chain, synced with Beamable inventory
5. **Updates**: Content updates don't require contract redeployment (metadata stored off-chain)

#### Best Practices

- **Naming**: Use clear, descriptive content type names (e.g., "weapon", "armor", "potion")
- **Decimals**: For FTs, use 7 decimals maximum (Stellar limit)
- **Images**: Use IPFS or permanent URLs for NFT images
- **Custom Properties**: Use for dynamic metadata that may vary per instance
- **Validation**: Add validators to prevent invalid content from being published
- **Versioning**: Consider content type names carefully - they're used in contract deployment

#### Testing Content

1. Create test content
2. Run microservice locally: `dotnet beam services run`
3. Grant test item to player
4. Verify contract deployment in microservice logs
5. Check Stellar testnet with [Stellar Laboratory](https://laboratory.stellar.org/)

#### Learn More

For detailed information about Beamable's content system:

- [Content Manager Overview](https://docs.beamable.com/docs/content-manager-overview)
- [Custom Content Types](https://docs.beamable.com/docs/content-custom-content)
- [Content Guide](https://docs.beamable.com/docs/content-guide)

### Building

```bash
# Build entire solution
dotnet build beamableServices.sln
```

### Running Locally

```bash
cd services/StellarFederation
dotnet tool restore //one-time operation 
dotnet beam login --save-to-file //one-time operation
dotnet beam services run
```

### Deployment

The service is deployed via Docker using Beamable CLI:

```bash
dotnet beam deploy plan
dotnet beam deploy release --latest-plan -q
```

The custom Dockerfile includes:
- .NET 8.0 runtime
- Rust toolchain (for contract compilation)
- Stellar CLI
- libsodium (for cryptographic operations)

## Development

### Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| Beamable.Microservice.Runtime | Auto-synced | Beamable SDK |
| stellar-dotnet-sdk | 14.0.1 | Stellar blockchain integration |
| Handlebars.Net | 2.1.6 | Contract template engine |
| Microsoft.Extensions.Caching.Memory | 9.0.9 | In-memory caching |
| Microsoft.Extensions.Hosting | 9.0.1 | Background service hosting |

### Service Registration

Services are auto-discovered via reflection:

1. All types implementing `IService` are registered as singletons
2. Endpoints implementing `IEndpoint` are auto-registered
3. Registration happens in `StellarFederation.Configure()` method

### Common Patterns

- Use `BeamableLogger` for logging, not `Console`
- Services injected via `Provider.GetService<T>()`
- Client-callable methods use `[ClientCallable]` attribute
- Admin-only operations use `[AdminOnlyCallable]` attribute

### Testing

- **Testnet**: Use Stellar friendbot to fund test accounts
- **Async Processing**: Transaction results arrive asynchronously via background service
- **Fees**: Ensure accounts have sufficient XLM for transaction fees
- **Locks**: Distributed locks have TTL; long operations may lose locks

## Performance Characteristics

### Transaction Batching

The service optimizes blockchain operations through intelligent batching:

- **Native Transfers**: Up to 100 XLM transfers per transaction
- **Contract Calls**: Batched based on gas limits and complexity
- **Work Groups**: Transactions grouped by contract address for parallel processing
- **Lock Granularity**: Distributed locks per work group, not global

### Caching Strategy

- **Account Data**: Stellar account info cached with TTL
- **Contract Addresses**: Deployed contract addresses cached indefinitely
- **Sequence Numbers**: Cached and synchronized with blockchain
- **Global State**: Cross-service cache for shared data

## Error Handling

Custom exception types inherit from `MicroserviceException`:

| Exception | Use Case |
|-----------|----------|
| `ConfigurationException` | Invalid realm configuration |
| `StellarServiceException` | Blockchain interaction failures |
| `TransactionException` | Transaction processing errors |
| `ContractException` | Smart contract deployment/execution errors |

## Security Considerations

- **Key Encryption**: Private keys encrypted at rest using AES-256
- **Access Control**: Beamable authentication required for all endpoints
- **Admin Operations**: Contract deployment restricted to admin users
- **Audit Logging**: All blockchain operations logged for audit trails

## Monitoring & Observability

- **Transaction Status**: Real-time status updates in MongoDB
- **Blockchain Events**: Monitored via Horizon API streaming
- **Error Tracking**: Failed transactions logged with full context
- **Performance Metrics**: Available through Beamable dashboard

## Resources

- [Beamable Documentation](https://help.beamable.com/WebSDK-Latest/)
- [Stellar Documentation](https://developers.stellar.org/)
- [Soroban Smart Contracts](https://soroban.stellar.org/)
- [IFederatedLogin Interface](https://github.com/beamable/FederatedAuthentication)
- [IFederatedInventory Interface](https://github.com/beamable/FederatedInventory)
