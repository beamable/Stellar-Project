#![no_std]
use soroban_sdk::{contract, contractimpl, Address, Env, String};
use stellar_tokens::fungible::{Base, FungibleToken};
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

        //Pre-mint if TotalSupply
        Base::mint(e, &initial_owner, {{TotalSupply}});

        // Set the contract owner
        ownable::set_owner(e, &initial_owner);
    }

    pub fn batch_approve(e: &Env, approvals: Vec<(Address, Address, i128, u32)>) {
        for (from, spender, amount, expiration_ledger) in approvals.into_iter() {
            Base::approve(e, &from, &spender, amount, expiration_ledger);
        }
    }

    pub fn batch_transfer(e: &Env, transfers: Vec<(Address, Address, Address, i128)>) {
            for (spender, from, to, amount) in transfers.into_iter() {
                Base::transfer(e, &spender, &from, &to, amount);
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
impl Ownable for {{toStructName Name}} {}