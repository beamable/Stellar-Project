using System; //Don't remove
using Beamable.StellarFederation.Caching; //Don't remove
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Server;
using Beamable.StellarFederation.Features.Contract.Storage;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Content;
using Beamable.StellarFederation.Features.Contract.CliWrapper;
using Beamable.StellarFederation.Features.Contract.Handlers;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.LockManager;
using Beamable.StellarFederation.Features.Stellar;

namespace Beamable.StellarFederation.Features.Contract;

public class ContractService : IService
{
    private readonly LockManagerService _lockManagerService;
    private readonly ContentContractHandlerResolver _contractHandlerResolver;
    private readonly ContractCollection _contractCollection;
    private readonly StellarService _stellarService;
    private readonly CliClient _cliClient;
    private readonly BeamContentService _beamContentService;

    public ContractService(SocketRequesterContext socketRequesterContext, LockManagerService lockManagerService, ContractCollection contractCollection, StellarService stellarService, ContentContractHandlerResolver contractHandlerResolver, CliClient cliClient, BeamContentService beamContentService)
    {
        _lockManagerService = lockManagerService;
        _contractCollection = contractCollection;
        _stellarService = stellarService;
        _contractHandlerResolver = contractHandlerResolver;
        _cliClient = cliClient;
        _beamContentService = beamContentService;
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

                foreach (var model in await _beamContentService.FetchFederationContentForContracts())
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

    public async Task<bool> UpsertContract<TContract>(TContract contract, string id) where TContract : ContractBase
    {
        return await _contractCollection.TryUpsert(contract, id);
    }
}