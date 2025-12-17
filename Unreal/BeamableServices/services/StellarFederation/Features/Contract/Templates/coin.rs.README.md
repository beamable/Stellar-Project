# Coin Smart Contract Template

## Overview

This is a **fungible token (FT) smart contract template** for Stellar (Soroban) that supports minting and burning operations. Contracts generated from this template are automatically created by the StellarFederation service based on Beamable content definitions.

## Content System Integration

### Interface Requirement
Content types using this template **must implement `IFtBase`** interface from `StellarFederationCommon.FederationContent`:

```csharp
public interface IFtBase
{
    string Name { get; }          // Token name
    string Symbol { get; }        // Token symbol (e.g., "COIN")
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

## Contract Functions

### Constructor
```rust
pub fn __constructor(e: &Env, initial_owner: Address)
```
Initializes the contract with token metadata and sets the initial owner.

**Parameters:**
- `initial_owner` - Address that will own the contract and have admin privileges

**Actions:**
- Sets token metadata (decimals, description, symbol)
- Assigns contract ownership to the specified address

---

### Batch Mint (Owner Only)
```rust
pub fn batch_mint(e: &Env, recipients: Vec<(Address, i128)>)
```
Mints new tokens to multiple recipients in a single transaction. **Only callable by contract owner.**

**Parameters:**
- `recipients` - Vector of tuples containing recipient addresses and amounts to mint

**Use Case:** Adding currency to player accounts when they earn or purchase tokens

---

### Batch Burn
```rust
pub fn batch_burn(e: &Env, froms: Vec<(Address, i128)>)
```
Burns (destroys) tokens from multiple addresses in a single transaction.

**Parameters:**
- `froms` - Vector of tuples containing addresses and amounts to burn

**Use Case:** Removing currency when players spend tokens or as part of deflationary mechanics

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

**Use Case:** Processing multiple player-to-player trades or rewards in one blockchain transaction

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

### FungibleBurnable
Adds burning functionality to the fungible token standard.

### Ownable
Provides ownership management with:
- `owner()` - Get current owner
- `transfer_ownership()` - Transfer ownership to new address
- `renounce_ownership()` - Remove owner (make contract immutable)

## Usage in StellarFederation Service

1. Define a content type in Beamable that implements `IFtBase`
2. The service automatically detects FT content and selects this template
3. Template variables are populated from content properties
4. Contract is compiled to WASM using Stellar CLI
5. Compiled contract is deployed to Stellar blockchain
6. Contract address is stored and used for inventory operations

## Key Differences from Gold Template

- **Supports minting**: New tokens can be created after deployment (by owner)
- **Supports burning**: Tokens can be destroyed to reduce supply
- **No pre-mint**: Total supply is not fixed at deployment
- **Use case**: Tokens with dynamic supply that can be earned/spent over time
