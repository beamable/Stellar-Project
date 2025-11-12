#![no_std]
use soroban_sdk::{contract, contractimpl, Address, Env, String};
use stellar_tokens::fungible::{burnable::FungibleBurnable, Base, FungibleToken};
use stellar_access::ownable::{self as ownable };
use stellar_macros::{default_impl, only_owner};

#[contract]
pub struct {{toStructName Name}};

#[contractimpl]
impl {{toStructName Name}} {
    pub fn __constructor(e: &Env, initial_owner: Address) {
        // Set token metadata
        Base::set_metadata(
            e,
            {{Decimals}}, // decimals
            String::from_str(e, "{{Description}}"),
            String::from_str(e, "{{Symbol}}"),
        );

        // Set the contract owner
        ownable::set_owner(e, &initial_owner);
    }

    #[only_owner]
    pub fn mint_tokens(e: &Env, to: Address, amount: i128) {
        // Mint tokens to the recipient
        Base::mint(e, &to, amount);
    }
}

#[default_impl]
#[contractimpl]
impl FungibleToken for {{toStructName Name}} {
    type ContractType = Base;
}

#[default_impl]
#[contractimpl]
impl FungibleBurnable for {{toStructName Name}} {}