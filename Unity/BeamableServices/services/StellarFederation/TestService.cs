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
using Beamable.StellarFederation.Features.Contract.Functions.Approval.Models;
using Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;
using Beamable.StellarFederation.Features.Contract.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.Scheduler.Storage.Modles;
using Beamable.StellarFederation.Features.Stellar;
using Beamable.StellarFederation.Features.Transactions;
using Beamable.StellarFederation.Features.Transactions.Models;
using MongoDB.Bson;

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

    public async Task<string> Test()
    {
        await _blockProcessor.Handle();
        return "";
    }
}