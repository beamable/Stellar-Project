using System;
using System.Collections.Generic;
using System.Linq; //Don't remove
using Beamable.StellarFederation.Caching; //Don't remove
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Server;
using Beamable.StellarFederation.Features.Contract.Storage;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Content;
using Beamable.StellarFederation.Features.Contract.CliWrapper;
using Beamable.StellarFederation.Features.Contract.Exceptions;
using Beamable.StellarFederation.Features.Contract.Handlers;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.LockManager;
using Beamable.StellarFederation.Features.Stellar;
using Beamable.StellarFederation.Features.WalletManager;

namespace Beamable.StellarFederation.Features.Contract;

public class ContractService : IService
{
    private readonly LockManagerService _lockManagerService;
    private readonly ContentContractHandlerResolver _contractHandlerResolver;
    private readonly ContractCollection _contractCollection;
    private readonly StellarService _stellarService;
    private readonly CliClient _cliClient;
    private readonly BeamContentService _beamContentService;
    private readonly WalletManagerService2 _walletManagerService;

    public ContractService(SocketRequesterContext socketRequesterContext, LockManagerService lockManagerService, ContractCollection contractCollection, StellarService stellarService, ContentContractHandlerResolver contractHandlerResolver, CliClient cliClient, BeamContentService beamContentService, WalletManagerService2 walletManagerService)
    {
        _lockManagerService = lockManagerService;
        _contractCollection = contractCollection;
        _stellarService = stellarService;
        _contractHandlerResolver = contractHandlerResolver;
        _cliClient = cliClient;
        _beamContentService = beamContentService;
        _walletManagerService = walletManagerService;
        SubscribeContentUpdateEvent(socketRequesterContext);
    }

    private void SubscribeContentUpdateEvent(SocketRequesterContext socketRequesterContext)
    {
#if !DEBUG
        socketRequesterContext.Subscribe<object>(Constants.Features.Services.CONTENT_UPDATE_EVENT, _ =>
        {
            Task.Run(async () =>
            {
                try
                {
                    BeamableLogger.Log("Content was updated");
                    GlobalCache.Remove(BeamContentService.FederationContentLocal);
                    await InitializeContentContracts();
                }
                catch (Exception ex)
                {
                    BeamableLogger.LogError($"Error during content update: {ex}");
                }
            });
        });
#endif
    }

    public async Task InitializeContentContracts()
    {
        try
        {
            _ = Extensions.TaskExtensions.RunAsyncBlock(async () =>
            {
                if (!await _lockManagerService.AcquireLock(AccountsService.RealmAccountName)) return;

                await _stellarService.Initialize();
                _cliClient.Initialize();

                var models = (await _beamContentService.FetchFederationContentForContracts()).ToList();
                await _walletManagerService.CreateContractWallets(models.Select(m => m.Key).ToList());

                BeamableLogger.Log("Waiting on contract wallets...");
                await Task.Delay(TimeSpan.FromSeconds(5));

                foreach (var model in models)
                {
                    var handler = _contractHandlerResolver.Resolve(model);
                    await handler.HandleContract(model);
                }

                BeamableLogger.Log("All contracts initialized.");
            });
        }
        finally
        {
            await _lockManagerService.ReleaseLock(AccountsService.RealmAccountName);
        }
    }

    public async Task<TContract?> GetByContent<TContract>(string contentId) where TContract : ContractBase
    {
        return await _contractCollection.GetByContentId<TContract>(contentId);
    }

    public async Task<TContract> GetByContentId<TContract>(string contentId) where TContract : ContractBase
    {
        var contract = await _contractCollection.GetByContentId<TContract>(contentId);
        if (contract is null)
            throw new ContractException($"Contract for {contentId} don't exist.");
        return contract;
    }

    public async Task<bool> UpsertContract<TContract>(TContract contract, string id) where TContract : ContractBase
    {
        return await _contractCollection.TryUpsert(contract, id);
    }
}