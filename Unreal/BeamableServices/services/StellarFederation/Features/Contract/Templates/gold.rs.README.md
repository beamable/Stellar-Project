# Gold Smart Contract Template

## Overview

This is a **fungible token (FT) smart contract template** for Stellar (Soroban) with a **fixed total supply**. This template is designed for tokens that have a predetermined supply that is fully minted at deployment. Contracts generated from this template are automatically created by the StellarFederation service based on Beamable content definitions.

## Content System Integration

### Interface Requirement
Content types using this template **must implement `IFtBase`** interface from `StellarFederationCommon.FederationContent`:

```csharp
public interface IFtBase
{
    string Name { get; }          // Token name
    string Symbol { get; }        // Token symbol (e.g., "GOLD")
    int Decimals { get; }         // Number of decimal places
    string Image { get; }         // Token image URL
    string Description { get; }   // Token description
}
```

### Template Variables
The template uses Handlebars syntax for parameterization:
- `{{toStructName Name}}` - Contract struct name derived from content name
- `{{Decimals}}` - Number of decimal places for the token
- `{{Description}}` - Token description
- `{{Symbol}}` - Token symbol/ticker
- `{{TotalSupply}}` - Total supply minted at contract creation

## Contract Functions

### Constructor
```rust
pub fn __constructor(e: &Env, initial_owner: Address)
```
Initializes the contract with token metadata, **pre-mints the entire total supply** to the owner, and sets contract ownership.

**Parameters:**
- `initial_owner` - Address that will own the contract and receive all tokens

**Actions:**
- Sets token metadata (decimals, description, symbol)
- Mints the entire `TotalSupply` to the initial owner
- Assigns contract ownership to the specified address

**Important:** This is the only time tokens are created. No minting function exists post-deployment.

---

### Batch Approve
```rust
pub fn batch_approve(e: &Env, approvals: Vec<(Address, Address, i128, u32)>)
```
Approves spending allowances for multiple spenders in a single transaction.

**Parameters:**
- `approvals` - Vector of tuples containing:
  - `from` - Token owner address
  - `spender` - Address being approved to spend
  - `amount` - Amount approved for spending
  - `expiration_ledger` - Ledger number when approval expires

**Use Case:** Allowing smart contracts or other addresses to spend tokens on behalf of users

---

### Batch Transfer
```rust
pub fn batch_transfer(e: &Env, transfers: Vec<(Address, Address, i128)>)
```
Transfers tokens from multiple senders to multiple recipients in a single transaction.

**Parameters:**
- `transfers` - Vector of tuples containing:
  - `from` - Sender address
  - `to` - Recipient address
  - `amount` - Amount to transfer

**Use Case:** Processing multiple player-to-player trades, rewards, or distributions in one blockchain transaction

---

### Batch Transfer From
```rust
pub fn batch_transfer_from(e: &Env, transfers: Vec<(Address, Address, Address, i128)>)
```
Transfers tokens using pre-approved allowances in batch.

**Parameters:**
- `transfers` - Vector of tuples containing:
  - `spender` - Address executing the transfer (must have allowance)
  - `from` - Token owner address
  - `to` - Recipient address
  - `amount` - Amount to transfer

**Use Case:** Smart contracts executing approved transfers on behalf of users

---

## Implemented Traits

### FungibleToken
Implements standard fungible token interface with methods like:
- `balance()` - Get token balance
- `transfer()` - Transfer tokens
- `allowance()` - Check spending allowance
- `total_supply()` - Get total token supply

### Ownable
Provides ownership management with:
- `owner()` - Get current owner
- `transfer_ownership()` - Transfer ownership to new address
- `renounce_ownership()` - Remove owner (make contract immutable)

## Usage in StellarFederation Service

1. Define a content type in Beamable that implements `IFtBase`
2. The service automatically detects FT content and selects this template
3. Template variables are populated from content properties (including TotalSupply)
4. Contract is compiled to WASM using Stellar CLI
5. Compiled contract is deployed to Stellar blockchain
6. Contract address is stored and used for inventory operations

## Key Differences from Coin Template

- **No minting function**: All tokens are created at deployment
- **No burning function**: Tokens cannot be destroyed (fixed supply)
- **Pre-minted supply**: Entire supply is minted to owner on deployment
- **Use case**: Premium currencies, limited edition tokens, or assets with fixed scarcity

## Design Philosophy

This template implements a "gold standard" token model:
- **Scarcity**: Total supply is known and cannot increase
- **Immutability**: No inflation mechanism exists post-deployment
- **Predictability**: Total supply is transparent and permanent

Perfect for:
- Premium in-game currencies (like "Gold" or "Gems")
- Limited edition assets
- Tokens representing real-world value with fixed scarcity
- Deflationary token models (if combined with burning mechanisms in game logic)
