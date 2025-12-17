# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Beamable microservices solution that integrates Stellar blockchain functionality with Beamable's federated inventory system. The solution consists of three projects:

- **StellarFederation** (service): Main microservice implementing federated login and inventory with Stellar blockchain integration
- **StellarFederationCommon** (netstandard2.1): Shared types and constants copied to linked Unity projects
- **StellarFederationStorage** (storage): MongoDB storage object for persistent data

The architecture implements federated inventory using Stellar smart contracts (Soroban) and integrates with Stellar's blockchain for managing in-game currencies and items as on-chain assets.

## Build & Development Commands

### Building
```bash
# Build entire solution
dotnet build beamableServices.sln

# Build specific project
dotnet build services/StellarFederation/StellarFederation.csproj
dotnet build services/StellarFederationCommon/StellarFederationCommon.csproj
dotnet build services/StellarFederationStorage/StellarFederationStorage.csproj

# Build for release
dotnet build beamableServices.sln -c Release
```

### Running
```bash
# Run the microservice (from StellarFederation directory)
cd services/StellarFederation
dotnet run

# Run with specific configuration
dotnet run --project services/StellarFederation/StellarFederation.csproj
```

### Docker
The service is deployed via Docker with a custom Dockerfile that includes:
- .NET 8.0 runtime
- Rust toolchain (for Stellar contract compilation)
- Stellar CLI
- libsodium for cryptographic operations

Build command is handled by Beamable CLI.

## Project Dependencies

### Target Frameworks
- **StellarFederation**: net8.0 (LTS until 2026)
- **StellarFederationCommon**: netstandard2.1 (Unity-compatible)
- **StellarFederationStorage**: net8.0

### Key NuGet Packages
- `Beamable.Microservice.Runtime` - Beamable SDK (version synced from .config/dotnet-tools.json)
- `stellar-dotnet-sdk` (v14.0.1) - Stellar blockchain integration
- `Handlebars.Net` (v2.1.6) - Template engine for smart contract generation
- `Microsoft.Extensions.Caching.Memory` (v9.0.9) - In-memory caching
- `Microsoft.Extensions.Hosting` (v9.0.1) - Background service hosting

## Architecture

### Service Structure

The main microservice (`StellarFederation`) implements `IFederatedInventory<StellarWeb3Identity>` to provide:
- Federated authentication using Stellar wallet addresses
- Blockchain-backed inventory management
- Smart contract deployment and interaction
- Background transaction processing

### Key Features (services/StellarFederation/Features/)

- **Accounts**: Manages Stellar wallet creation, encryption, and authentication
- **BlockProcessor**: Monitors Stellar blockchain for events using Horizon and Soroban APIs
- **Contract**: Handles smart contract deployment, compilation (Rust templates), and interaction
- **Inventory**: Maps Beamable inventory operations to Stellar asset transfers
- **Transactions**: Queues and batches blockchain transactions for efficiency
- **WalletManager**: Manages working wallets for transaction processing
- **LockManager**: Distributed locking for transaction coordination

### Background Service

A separate hosted service (`StellarBackgroundService`) runs concurrently to process queued transactions:
- Fetches batched transactions from MongoDB
- Acquires distributed locks per work group
- Submits transactions to Stellar blockchain
- Implements exponential backoff for retries

### Endpoints (services/StellarFederation/Endpoints/)

Custom endpoints implementing `IEndpoint` interface:
- `AuthenticateEndpoint` - Wallet-based authentication
- `AuthenticateExternalEndpoint` - External identity linking
- `GetInventoryStateEndpoint` - Fetch blockchain inventory state
- `StartInventoryTransactionEndpoint` - Initiate inventory changes

### Configuration

Configuration is managed through Beamable's realm config system (namespace: `stellar-federation`):
- `StellarRpc` - Soroban RPC endpoint
- `StellarHorizon` - Horizon API endpoint
- `StellarNetwork` - "testnet" or "public"
- `NumberOfWorkingWallets` - Transaction processing pool size
- `MessageQueueBatchLimit` - Transaction batch size

Access via `Configuration` service injected through DI.

### Service Registration

Services are auto-discovered and registered via reflection:
- All types implementing `IService` are registered as singletons
- Endpoints implementing `IEndpoint` are auto-registered
- Registration happens in `StellarFederation.Configure()` method

### Storage Collections

MongoDB collections (accessed via `StellarFederationStorage`):
- `VaultCollection` - Encrypted wallet private keys
- `SequenceCollection` - Stellar account sequence numbers
- `ContractCollection` - Deployed contract addresses
- `InventoryTransactionCollection` - Pending inventory transactions
- `LockCollection` - Distributed locks
- `ExternalAuthCollection` - External authentication mappings

## Important Implementation Notes

### Beamable Version Synchronization
Projects use MSBuild logic to extract Beamable version from `.config/dotnet-tools.json` to keep SDK packages in sync with CLI version. This happens automatically unless running in Docker (checked via `DOTNET_RUNNING_IN_CONTAINER` environment variable).

### Initialization Flow
1. `MicroserviceBootstrapper.Prepare<StellarFederation>()` - CLI data injection
2. `Configure()` - Service registration via reflection
3. `Initialize()` - Validate config, create realm account, initialize contracts (non-DEBUG only)
4. Background service starts transaction processor

### Transaction Processing
- Client calls modify inventory through `IFederatedInventory` interface
- Transactions are queued to MongoDB with status tracking
- Background service claims work groups using distributed locks
- Multiple operations are batched into single Stellar transactions when possible
- Native transfers (XLM) and contract calls (custom tokens) use different batch limits

### Smart Contract Deployment
- Contract templates are Rust source files in `Features/Contract/Templates/`
- Templates use Handlebars for parameterization
- Contracts are compiled using Stellar CLI (extracted from `Tools/stellar.zip`)
- Compiled WASM is deployed to Stellar blockchain
- Contract addresses are stored in MongoDB for reuse

### Caching
- `Memoizer` class provides method result caching with TTL
- `GlobalCache` for cross-service state
- Used extensively for Stellar account data and contract addresses

### Error Handling
Custom exception types inherit from `MicroserviceException`:
- `ConfigurationException` - Invalid realm configuration
- `StellarServiceException` - Blockchain interaction failures
- `TransactionException` - Transaction processing errors
- `ContractException` - Smart contract deployment/execution errors

## Development Workflow

1. Modify service code in `services/StellarFederation/`
2. Shared types go in `StellarFederationCommon` (auto-copied to Unity via `CopyToLinkedProjects`)
3. Test locally with `dotnet run` or through Beamable CLI
4. Deploy via Beamable CLI which builds Docker image and pushes to Beamable cloud

### Common Development Patterns

- Services are injected via `Provider.GetService<T>()`
- Use `BeamableLogger` for logging, not `Console`
- Async methods return `Promise<T>` (Beamable's awaitable type), not `Task<T>`
- Client-callable methods use `[ClientCallable]` attribute
- Admin-only operations use `[AdminOnlyCallable]` attribute
- Federated interface methods are implemented explicitly

### Testing Considerations

- Stellar testnet requires funded accounts (via friendbot faucet)
- Transaction processing is asynchronous; results arrive via background service
- Smart contract operations require sufficient XLM for fees
- Distributed locks have TTL; long operations may lose locks
