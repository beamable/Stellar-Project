#![no_std]
use soroban_sdk::{contracttype, Address, contract, contractimpl, Env, Vec, String, Map, Symbol, symbol_short};
use stellar_access::ownable::{self as ownable, Ownable};
use stellar_macros::{default_impl, only_owner};
use stellar_tokens::non_fungible::{Base, burnable::NonFungibleBurnable, NonFungibleToken};

const METADATA_KEY: Symbol = symbol_short!("METADATA");
const MAX_URI_LEN: usize = 256;

#[contracttype]
pub enum DataKey {
    WalletTokens(Address), // Maps wallet -> Vec<u128>
}

#[contract]
pub struct {{toStructName Name}};

#[contractimpl]
impl {{toStructName Name}} {
    pub fn __constructor(e: &Env, initial_owner: Address) {
        let uri = String::from_str(e, "{{BaseUri}}");
        let name = String::from_str(e, "{{Name}}");
        let symbol = String::from_str(e, "{{toUpperCase Name}}");
        Base::set_metadata(e, uri, name, symbol);
        ownable::set_owner(e, &initial_owner);

        let metadata: Map<u32, String> = Map::new(e);
        e.storage().instance().set(&METADATA_KEY, &metadata);
    }

    #[only_owner]
    pub fn batch_mint(e: &Env, mints: Vec<(Address, u32, String)>) {
        let mut metadata: Map<u32, String> = e.storage()
            .instance()
            .get(&METADATA_KEY)
            .unwrap_or(Map::new(e));
        for (to, token_id, metadata_uri) in mints.into_iter() {
            Base::mint(e, &to, token_id);
            metadata.set(token_id, metadata_uri);
            let key = DataKey::WalletTokens(to.clone());
            let mut tokens: Vec<u32> = e.storage()
                .persistent()
                .get(&key)
                .unwrap_or(Vec::new(&e));

            tokens.push_back(token_id);
            e.storage().persistent().set(&key, &tokens);
        }
        e.storage().instance().set(&METADATA_KEY, &metadata);
    }

    pub fn batch_burn(e: &Env, froms: Vec<(Address, u32)>) {
        let mut metadata: Map<u32, String> = e.storage()
            .instance()
            .get(&METADATA_KEY)
            .unwrap_or(Map::new(e));
        for (from, token_id) in froms.into_iter() {
            Base::burn(e, &from, token_id);
            metadata.remove(token_id);
            let key = DataKey::WalletTokens(from.clone());
            let mut tokens: Vec<u32> = e.storage()
                .persistent()
                .get(&key)
                .unwrap_or(Vec::new(&e));
            if let Some(pos) = tokens.first_index_of(token_id) {
                tokens.remove(pos);
                e.storage().persistent().set(&key, &tokens);
            }
        }
        e.storage().instance().set(&METADATA_KEY, &metadata);
    }

    pub fn get_metadata(e: &Env, token_id: u32) -> String {
        let base_uri = Base::base_uri(e);
        let metadata: Option<Map<u32, String>> = e.storage()
        .instance()
        .get(&METADATA_KEY);

       match metadata {
            Some(m) => {
                match m.get(token_id) {
                    Some(suffix) => Self::compose_uri(e, base_uri, suffix),
                    None => String::from_str(e, "")
                }
            },
            None => String::from_str(e, "")
        }
    }

    #[only_owner]
    pub fn batch_update_metadata(e: &Env, updates: Vec<(u32, String)>) {
        let mut metadata: Map<u32, String> = e.storage()
            .instance()
            .get(&METADATA_KEY)
            .unwrap_or(Map::new(e));

        for (token_id, metadata_uri) in updates.into_iter() {
            metadata.set(token_id, metadata_uri);
        }
        e.storage().instance().set(&METADATA_KEY, &metadata);
    }

    fn compose_uri(e: &Env, base_uri: String, suffix: String) -> String {
        let base_len = base_uri.len() as usize;
        let suffix_len = suffix.len() as usize;

        if base_len > 0 && suffix_len > 0 {
            let uri = &mut [0u8; MAX_URI_LEN];
            base_uri.copy_into_slice(&mut uri[..base_len]);
            suffix.copy_into_slice(&mut uri[base_len..base_len + suffix_len]);
            String::from_bytes(e, &uri[..base_len + suffix_len])
        } else if base_len > 0 {
            base_uri
        } else {
            String::from_str(e, "")
        }
    }

    pub fn get_wallet_tokens(e: &Env, owner: Address) -> Vec<u32> {
        let key = DataKey::WalletTokens(owner);
        e.storage()
            .persistent()
            .get(&key)
            .unwrap_or(Vec::new(&e))
    }
}


#[contractimpl]
impl NonFungibleToken for {{toStructName Name}} {
    type ContractType = Base;

    fn token_uri(e: &Env, token_id: u32) -> String {
        let base_uri = Base::base_uri(&e);
        let metadata: Option<Map<u32, String>> = e.storage()
            .instance()
            .get(&METADATA_KEY);

        match metadata {
            Some(m) => {
                match m.get(token_id) {
                    Some(suffix) => {{toStructName Name}}::compose_uri(&e, base_uri, suffix),
                    None => String::from_str(&e, "")
                }
            },
            None => String::from_str(&e, "")
        }
    }

    fn balance(e: &Env, account: Address) -> u32 {
        Base::balance(e, &account)
    }

    fn owner_of(e: &Env, token_id: u32) -> Address  {
        Base::owner_of(e, token_id)
    }

    fn transfer(e: &Env, from: Address, to: Address, token_id: u32)  {
        // Remove from sender's list
        let from_key = DataKey::WalletTokens(from.clone());
        let mut from_tokens: Vec<u32> = e.storage()
            .persistent()
            .get(&from_key)
            .unwrap_or(Vec::new(&e));

        if let Some(pos) = from_tokens.first_index_of(token_id) {
            from_tokens.remove(pos);
            e.storage().persistent().set(&from_key, &from_tokens);
        }

        // Add to receiver's list
        let to_key = DataKey::WalletTokens(to.clone());
        let mut to_tokens: Vec<u32> = e.storage()
            .persistent()
            .get(&to_key)
            .unwrap_or(Vec::new(&e));

        to_tokens.push_back(token_id);
        e.storage().persistent().set(&to_key, &to_tokens);
        Base::transfer(e, &from, &to, token_id);
    }

    fn transfer_from(e: &Env, spender: Address, from: Address, to: Address, token_id: u32)  {
        // Remove from sender's list
        let from_key = DataKey::WalletTokens(from.clone());
        let mut from_tokens: Vec<u32> = e.storage()
            .persistent()
            .get(&from_key)
            .unwrap_or(Vec::new(&e));

        if let Some(pos) = from_tokens.first_index_of(token_id) {
            from_tokens.remove(pos);
            e.storage().persistent().set(&from_key, &from_tokens);
        }

        // Add to receiver's list
        let to_key = DataKey::WalletTokens(to.clone());
        let mut to_tokens: Vec<u32> = e.storage()
            .persistent()
            .get(&to_key)
            .unwrap_or(Vec::new(&e));

        to_tokens.push_back(token_id);
        e.storage().persistent().set(&to_key, &to_tokens);
        Base::transfer_from(e, &spender, &from, &to, token_id);
    }

    fn approve(e: &Env, approver: Address, approved: Address, token_id: u32, live_until_ledger: u32)  {
        Base::approve(e, &approver, &approved, token_id, live_until_ledger);
    }

    fn approve_for_all(e: &Env, owner: Address, operator: Address, live_until_ledger: u32)  {
        Base::approve_for_all(e, &owner, &operator, live_until_ledger);
    }

    fn get_approved(e: &Env, token_id: u32) -> Option<Address> {
        Base::get_approved(e, token_id)
    }

    fn is_approved_for_all(e: &Env, owner: Address, operator: Address) -> bool {
        Base::is_approved_for_all(e, &owner, &operator)
    }

    fn name(e: &Env) -> String {
        Base::name(e)
    }

    fn symbol(e: &Env) -> String {
        Base::symbol(e)
    }
}

#[contractimpl]
impl NonFungibleBurnable for {{toStructName Name}} {
     fn burn(e: &Env, from: Address, token_id: u32) {
        let mut metadata: Map<u32, String> = e.storage()
            .instance()
            .get(&METADATA_KEY)
            .unwrap_or(Map::new(e));
        metadata.remove(token_id);
        e.storage().instance().set(&METADATA_KEY, &metadata);
        Base::burn(e, &from, token_id);
    }

    fn burn_from(e: &Env, spender: Address, from: Address, token_id: u32) {
        let mut metadata: Map<u32, String> = e.storage()
            .instance()
            .get(&METADATA_KEY)
            .unwrap_or(Map::new(e));
        metadata.remove(token_id);
        e.storage().instance().set(&METADATA_KEY, &metadata);
        Base::burn_from(e, &spender, &from, token_id);
    }
}

#[default_impl]
#[contractimpl]
impl Ownable for {{toStructName Name}} {}