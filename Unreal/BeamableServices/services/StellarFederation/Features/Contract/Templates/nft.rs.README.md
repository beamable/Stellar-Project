# NFT Smart Contract Template

## Overview

This is a **non-fungible token (NFT) smart contract template** for Stellar (Soroban). Each token is unique with its own metadata URI. Contracts generated from this template are automatically created by the StellarFederation service based on Beamable content definitions.

## Content System Integration

### Interface Requirement
Content types using this template **must implement `INftBase`** interface from `StellarFederationCommon.FederationContent`:

```csharp
public interface INftBase
{
    string Name { get; }                                    // Token collection name
    string Image { get; }                                   // Default image URL
    string Description { get; }                             // Collection description
    SerializableDictionaryStringToString CustomProperties { get; } // Additional properties
}
```

### Template Variables
The template uses Handlebars syntax for parameterization:
- `{{toStructName Name}}` - Contract struct name derived from content name
- `{{Name}}` - NFT collection name
- `{{toUpperCase Name}}` - Token symbol (uppercase version of name)
- `{{BaseUri}}` - Base URI for token metadata (suffixes are appended per token)

## Contract Functions

### Constructor
```rust
pub fn __constructor(e: &Env, initial_owner: Address)
```
Initializes the NFT collection with metadata and sets contract ownership.

**Parameters:**
- `initial_owner` - Address that will own the contract and have admin privileges

**Actions:**
- Sets collection metadata (base URI, name, symbol)
- Assigns contract ownership
- Initializes empty metadata storage map

---

### Batch Mint (Owner Only)
```rust
pub fn batch_mint(e: &Env, mints: Vec<(Address, u32, String)>)
```
Mints new NFTs to multiple recipients in a single transaction. **Only callable by contract owner.**

**Parameters:**
- `mints` - Vector of tuples containing:
  - `to` - Recipient address
  - `token_id` - Unique token identifier
  - `metadata_uri` - URI suffix for this token's metadata (appended to base URI)

**Actions:**
- Mints NFT to specified address
- Stores metadata URI for the token
- Adds token ID to owner's token list for quick lookup

**Use Case:** Minting items to players when they acquire new assets

---

### Batch Burn
```rust
pub fn batch_burn(e: &Env, froms: Vec<(Address, u32)>)
```
Burns (destroys) NFTs from multiple addresses in a single transaction.

**Parameters:**
- `froms` - Vector of tuples containing:
  - `from` - Owner address
  - `token_id` - Token ID to burn

**Actions:**
- Burns the NFT
- Removes token metadata from storage
- Removes token ID from owner's token list

**Use Case:** Removing items when consumed, broken, or traded in for other assets

---

### Get Metadata
```rust
pub fn get_metadata(e: &Env, token_id: u32) -> String
```
Retrieves the complete metadata URI for a specific token.

**Parameters:**
- `token_id` - Token identifier

**Returns:** Full metadata URI (base URI + token-specific suffix)

**Use Case:** Fetching token metadata for display in game UI or marketplaces

---

### Batch Update Metadata (Owner Only)
```rust
pub fn batch_update_metadata(e: &Env, updates: Vec<(u32, String)>)
```
Updates metadata URIs for multiple tokens. **Only callable by contract owner.**

**Parameters:**
- `updates` - Vector of tuples containing:
  - `token_id` - Token to update
  - `metadata_uri` - New metadata URI suffix

**Actions:**
- Updates metadata URIs in storage
- Emits event with count of updated tokens

**Use Case:** Updating item properties, revealing NFTs, or fixing metadata issues

---

### Get Wallet Tokens
```rust
pub fn get_wallet_tokens(e: &Env, owner: Address) -> Vec<u32>
```
Retrieves all token IDs owned by a specific address.

**Parameters:**
- `owner` - Address to query

**Returns:** Vector of token IDs owned by the address

**Use Case:** Displaying a player's entire NFT inventory efficiently

---

## Standard NFT Interface Functions

The contract implements the full `NonFungibleToken` trait:

### Token URI
```rust
fn token_uri(e: &Env, token_id: u32) -> String
```
Returns the metadata URI for a token (combines base URI with stored suffix).

### Balance
```rust
fn balance(e: &Env, account: Address) -> u32
```
Returns the number of NFTs owned by an address.

### Owner Of
```rust
fn owner_of(e: &Env, token_id: u32) -> Address
```
Returns the owner address of a specific token.

### Transfer
```rust
fn transfer(e: &Env, from: Address, to: Address, token_id: u32)
```
Transfers an NFT from one address to another. Automatically updates wallet token lists.

### Transfer From
```rust
fn transfer_from(e: &Env, spender: Address, from: Address, to: Address, token_id: u32)
```
Transfers an NFT using a pre-approved allowance. Automatically updates wallet token lists.

### Approve
```rust
fn approve(e: &Env, approver: Address, approved: Address, token_id: u32, live_until_ledger: u32)
```
Approves another address to transfer a specific token.

### Approve For All
```rust
fn approve_for_all(e: &Env, owner: Address, operator: Address, live_until_ledger: u32)
```
Approves an operator to manage all of the owner's tokens.

### Get Approved
```rust
fn get_approved(e: &Env, token_id: u32) -> Option<Address>
```
Returns the approved address for a token, if any.

### Is Approved For All
```rust
fn is_approved_for_all(e: &Env, owner: Address, operator: Address) -> bool
```
Checks if an operator is approved to manage all tokens of an owner.

### Name / Symbol
```rust
fn name(e: &Env) -> String
fn symbol(e: &Env) -> String
```
Returns collection name and symbol.

---

## Implemented Traits

### NonFungibleToken
Standard NFT interface with transfer, approval, and query functions.

### NonFungibleBurnable
Adds burning functionality:
- `burn()` - Burn your own token
- `burn_from()` - Burn using approval

### Ownable
Ownership management for admin functions.

---

## Storage Architecture

### Persistent Storage
- **WalletTokens**: Maps `Address -> Vec<u32>` to track all tokens owned by each wallet
  - Enables efficient "get all tokens for address" queries
  - Updated on mint, burn, and transfer operations

### Instance Storage
- **METADATA**: Maps `u32 -> String` to store metadata URI suffixes per token
  - Combined with base URI to form complete metadata URL
  - Updated on mint, burn, and metadata update operations

### URI Composition
Metadata URIs are composed as: `{BaseUri}{metadata_uri_suffix}`
- Base URI is set at deployment
- Each token has a unique suffix stored in contract storage
- Maximum URI length is 256 characters

---

## Usage in StellarFederation Service

1. Define a content type in Beamable that implements `INftBase`
2. The service automatically detects NFT content and selects this template
3. Template variables are populated from content properties
4. Contract is compiled to WASM using Stellar CLI
5. Compiled contract is deployed to Stellar blockchain
6. Contract address is stored and used for inventory operations

---

## Design Considerations

### Efficient Inventory Queries
The `get_wallet_tokens()` function and WalletTokens storage enable:
- Fast retrieval of all tokens owned by a player
- No need to iterate through all token IDs
- Efficient inventory display in game clients

### Metadata Flexibility
- Base URI can point to IPFS, centralized server, or any metadata endpoint
- Individual token metadata can be updated without changing base URI
- Supports revealed/unrevealed NFT mechanics

### Gas Optimization
- Batch operations reduce transaction costs
- Metadata stored as URI suffixes rather than full URLs
- Token lists stored per wallet for efficient queries

### Owner Controls
- Only owner can mint new tokens
- Only owner can update metadata
- Supports centralized game asset management while maintaining blockchain provenance
