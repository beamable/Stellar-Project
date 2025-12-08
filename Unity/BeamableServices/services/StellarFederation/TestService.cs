using System;
using System.Threading.Tasks;
using Beamable.Common.Dependencies;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.BlockProcessor;
using Beamable.StellarFederation.Features.BlockProcessor.Decoder;
using Beamable.StellarFederation.Features.BlockProcessor.Handlers;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.Content;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.CliWrapper;
using Beamable.StellarFederation.Features.Contract.Functions.Account.Models;
using Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;
using Beamable.StellarFederation.Features.Contract.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.Scheduler.Storage.Modles;
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
    private readonly ContractService _contractService;
    private readonly StellarRpcClient _stellarRpcClient;
    private readonly Configuration _configuration;
    private readonly SorobanBlockProcessor _blockProcessor;
    private readonly HorizonBlockProcessor _horizonBlockProcessor;
    private readonly CliClient _cliClient;
    private readonly BeamContentService _beamContentService;


    public TestService(ContractProxy contractProxy, AccountsService accountsService, TransactionManager transactionManager, StellarService stellarService, CreateAccountBlockHandler createAccountBlockHandler, ContractService contractService, StellarRpcClient stellarRpcClient, Configuration configuration, SorobanBlockProcessor blockProcessor, HorizonBlockProcessor horizonBlockProcessor, CliClient cliClient, BeamContentService beamContentService)
    {
        _contractProxy = contractProxy;
        _accountsService = accountsService;
        _transactionManager = transactionManager;
        _stellarService = stellarService;
        _createAccountBlockHandler = createAccountBlockHandler;
        _contractService = contractService;
        _stellarRpcClient = stellarRpcClient;
        _configuration = configuration;
        _blockProcessor = blockProcessor;
        _horizonBlockProcessor = horizonBlockProcessor;
        _cliClient = cliClient;
        _beamContentService = beamContentService;
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

    public async Task Test()
    {
        var realmAccount = await _accountsService.GetOrCreateRealmAccount();
        var block = new Block
        {
            Network = await _configuration.StellarRpc,
            Api = StellarSettings.SorobanApi,
            Cursor = "",
            BlockNumber = 1910621
        };
        // var logs = await _stellarService.GetSorobanLogs(block, ["CADXVUHVL6OVDO56DPIUQNVC3AZTUF2FKNP34DTP4ER7AVGVGOIIHXZB"]);
        // var transferDecoder = new SorobanEventDecoder<MintEventDto>("mint");
        // var transferEvents = transferDecoder.DecodeEvents(logs.Events);
        //await _blockProcessor.Handle();
        await _contractService.InitializeContentContracts();
        var ie = 0;

        //await _contractService.InitializeContentContracts();
        //await _createAccountBlockHandler.Handle(1806810, 1806810);
        // Task task1 = _accountsService.CreateNewAccount("123");
        // Task task2 = _accountsService.CreateNewAccount("1234");
        //
        // await Task.WhenAll(task1, task2);
        // var contract = await _contractService.GetByContentId<CoinContract>("currency.coin.beam_coin");
        // await _stellarRpcClient.InvokeContractAsync(contract.Address, new BalanceFunctionMessage("currency.coin.beam_coin",
        //     "GCETJ47OHSHT4VFWSAK3SYHXTVRS3U72MKMJH7JFSV5DSASPMJTP24OU"));

        var i = 0;
    }

    public async Task<string> Test2()
    {
        await _blockProcessor.Handle();
        return "";
    }
}