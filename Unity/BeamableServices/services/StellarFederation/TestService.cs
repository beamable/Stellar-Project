using System;
using System.Threading.Tasks;
using Beamable.Common.Dependencies;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.BlockProcessor.Handlers;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Functions.Account.Models;
using Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;
using Beamable.StellarFederation.Features.Stellar;
using Beamable.StellarFederation.Features.Transactions;
using Beamable.StellarFederation.Features.Transactions.Models;

namespace Beamable.StellarFederation;

public class TestService : IService
{
    private readonly ContractProxy _contractProxy;
    private readonly AccountsService _accountsService;
    private readonly TransactionManager _transactionManager;
    private readonly StellarService _stellarService;
    private readonly CreateAccountBlockHandler _createAccountBlockHandler;


    public TestService(ContractProxy contractProxy, AccountsService accountsService, TransactionManager transactionManager, StellarService stellarService, CreateAccountBlockHandler createAccountBlockHandler)
    {
        _contractProxy = contractProxy;
        _accountsService = accountsService;
        _transactionManager = transactionManager;
        _stellarService = stellarService;
        _createAccountBlockHandler = createAccountBlockHandler;
    }

    // public async Task Test(string hash)
    // {
    //     var realmAccount = await _accountsService.GetOrCreateRealmAccount();
    //
    //     // var tx = await _stellarService.GetStellarTransaction(hash);
    //     // var i = 0;
    //     var invTx = Guid.NewGuid().ToString();
    //     var tx = await _transactionManager.StartTransaction(new NewCustomTransaction(0, realmAccount.Address, invTx,
    //         "testMint", "mint"));
    //     await _transactionManager.RunAsyncBlock(tx, invTx, async () =>
    //     {
    //         await _contractProxy.CoinBatchMint(new MintCoinFunctionMessage(tx, "currency.coin.beam_coin",
    //             ["GCVEPND367W4HISKCUOS4A4FYDUAOPBSRT6J7AY5FWUTKNF6VEIO2UHF"], [7]));
    //     });
    //
    //     var invTx2= Guid.NewGuid().ToString();
    //     var tx2 = await _transactionManager.StartTransaction(new NewCustomTransaction(0, realmAccount.Address, invTx2,
    //         "testMint", "mint"));
    //     await _transactionManager.RunAsyncBlock(tx2, invTx2, async () =>
    //     {
    //         await _contractProxy.CoinBatchMint(new MintCoinFunctionMessage(tx2, "currency.coin.beam_coin",
    //             ["GCVEPND367W4HISKCUOS4A4FYDUAOPBSRT6J7AY5FWUTKNF6VEIO2UHF"], [8]));
    //     });
    //
    // }
    //

    public async Task Test(long block)
    {
        var realmAccount = await _accountsService.GetOrCreateRealmAccount();
        await _createAccountBlockHandler.Handle(1806810, 1806810);
        // Task task1 = _accountsService.CreateNewAccount("123");
        // Task task2 = _accountsService.CreateNewAccount("1234");
        //
        // await Task.WhenAll(task1, task2);
        var i = 0;
    }
}