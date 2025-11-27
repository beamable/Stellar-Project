#![no_std]
use soroban_sdk::{contract, contractimpl, Address, Env, String};
use stellar_tokens::fungible::{burnable::FungibleBurnable, Base, FungibleToken};
use stellar_access::ownable::{self as ownable };
use stellar_macros::{default_impl, only_owner};
use soroban_sdk::Vec;

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
    pub fn batch_mint(e: &Env, recipients: Vec<(Address, i128)>) {
        for (to, amount) in recipients.into_iter() {
            Base::mint(e, &to, amount);
        }
    }

    pub fn batch_burn(e: &Env, froms: Vec<(Address, i128)>) {
        for (from, amount) in froms.into_iter() {
            Base::burn(e, &from, amount);
        }
    }

    pub fn batch_approve(e: &Env, approvals: Vec<(Address, Address, i128, u32)>) {
        for (from, spender, amount, expiration_ledger) in approvals.into_iter() {
            Base::approve(e, &from, &spender, amount, expiration_ledger);
        }
    }

    pub fn batch_transfer_from(e: &Env, transfers: Vec<(Address, Address, Address, i128)>) {
        for (spender, from, to, amount) in transfers.into_iter() {
            Base::transfer_from(e, &spender, &from, &to, amount);
        }
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