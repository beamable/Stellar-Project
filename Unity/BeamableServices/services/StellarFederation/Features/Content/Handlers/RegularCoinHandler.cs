using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Content;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Content.Handlers.Models;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.Stellar;
using Beamable.StellarFederation.Features.Transactions;
using Beamable.StellarFederation.Features.Transactions.Storage;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;

namespace Beamable.StellarFederation.Features.Content.Handlers;

public class RegularCoinHandler : IService, IContentHandler
{
    private readonly ContractService _contractService;
    private readonly AccountsService _accountsService;
    private readonly MintCollection _mintCollection;
    private readonly StellarService _stellarService;
    private readonly TransactionManager _transactionManager;

    public RegularCoinHandler(ContractService contractService, AccountsService accountsService, MintCollection mintCollection, StellarService stellarService, TransactionManager transactionManager)
    {
        _contractService = contractService;
        _accountsService = accountsService;
        _mintCollection = mintCollection;
        _stellarService = stellarService;
        _transactionManager = transactionManager;
    }

    // public async Task<BaseMessage?> ConstructMessage(string transaction, string wallet, InventoryRequest inventoryRequest, IContentObject contentObject) =>
    //     inventoryRequest.Amount switch
    //     {
    //         > 0 => await PositiveAmountMessage(transaction, wallet, "mint_tokens", inventoryRequest),
    //         < 0 => await NegativeAmountMessage(transaction, wallet, "burn", inventoryRequest),
    //         _ => null
    //     };
    //
    // private async Task<RegularCoinMintMessage> PositiveAmountMessage(string transaction, string wallet, string function, InventoryRequest inventoryRequest)
    // {
    //     var contract = await _contractService.GetByContentId<CoinContract>(inventoryRequest.ContentId);
    //     return new RegularCoinMintMessage
    //     {
    //         ContentId =  inventoryRequest.ContentId,
    //         ContractId = contract.Address,
    //         Function = function,
    //         PlayerWalletAddress = wallet,
    //         GamerTag = inventoryRequest.GamerTag,
    //         Amount = inventoryRequest.Amount
    //     };
    // }
    //
    // private async Task<RegularCoinBurnMessage?> NegativeAmountMessage(string transaction, string wallet, string function, InventoryRequest inventoryRequest)
    // {
    //     var contract = await _contractService.GetByContentId<CoinContract>(inventoryRequest.ContentId);
    //     var balance = 0;//await _suiApiService.GetCoinBalance(wallet, new CoinBalanceRequest(contract.PackageId, contract.Module));
    //     if (balance >= Math.Abs(inventoryRequest.Amount))
    //         return new RegularCoinBurnMessage
    //         {
    //             ContentId =  inventoryRequest.ContentId,
    //             ContractId = contract.Address,
    //             Function = function,
    //             PlayerWalletAddress = wallet,
    //             GamerTag = inventoryRequest.GamerTag,
    //             Amount = inventoryRequest.Amount
    //         };
    //
    //     await _transactionManager.AddChainTransaction(transaction, new ChainTransaction
    //     {
    //         Error = $"Insufficient funds for {inventoryRequest.ContentId}, balance is {0}, requested is {Math.Abs(inventoryRequest.Amount)}",
    //         Function = $"{nameof(RegularCoinHandler)}.{nameof(NegativeAmountMessage)}"
    //     });
    //     return null;
    // }

    public async Task PrepareTransactions(string transaction, string wallet, List<BaseMessage> messages)
    {
        if (messages.Count == 0) return;

        var lookup = messages.ToLookup(m => m.GetType());

        var mintMessages = lookup[typeof(RegularCoinMintMessage)].ToList();
        if (mintMessages.Count > 0)
        {
            await PreparePositiveAmountTransaction(transaction, mintMessages.Cast<RegularCoinMintMessage>().ToList());
        }

        // var burnMessages = lookup[typeof(RegularCoinBurnMessage)].ToList();
        // if (burnMessages.Count > 0)
        // {
        //     await SendNegativeMessage(transaction, burnMessages.Cast<RegularCoinBurnMessage>().ToList());
        // }
    }

    private async Task PreparePositiveAmountTransaction(string transaction, List<RegularCoinMintMessage> messages)
    {
        var realmAccount = await _accountsService.GetOrCreateRealmAccount();
        try
        {
            //var result = await _stellarService.MintRegularCurrencyTransaction(realmAccount, messages);
            // await transactionManager.AddChainTransaction(new ChainTransaction
            // {
            //     Hash = result.digest,
            //     Error = result.error,
            //     Function = $"{nameof(RegularCoinHandler)}.{nameof(PreparePositiveAmountTransaction)}",
            //     Data = messages.SerializeSelected(),
            //     Status = result.status,
            // });
            // if (result.status != "success")
            // {
            //     var message = $"{nameof(RegularCoinHandler)}.{nameof(PreparePositiveAmountTransaction)} failed with status {result.status}";
            //     BeamableLogger.LogError(message);
            //     await transactionManager.TransactionError(transaction, new Exception(message));
            // }
        }
        catch (Exception e)
        {
            var message =
                $"{nameof(RegularCoinHandler)}.{nameof(PreparePositiveAmountTransaction)} failed with error {e.Message}";
            BeamableLogger.LogError(message);
        }
    }
}