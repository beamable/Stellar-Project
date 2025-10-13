using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.Server;
using Beamable.StellarFederation.Features.Contract.Storage;
using Beamable.Server.Content;
using Beamable.StellarFederation.Caching;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Contract.CliWrapper;
using Beamable.StellarFederation.Features.Contract.Handlers;
using Beamable.StellarFederation.Features.Contract.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.LockManager;
using Beamable.StellarFederation.Features.Stellar;
using StellarFederationCommon;

namespace Beamable.StellarFederation.Features.Contract;

public class ContractService : IService
{
    private readonly LockManagerService _lockManagerService;
    private readonly ContentService _contentService;
    private readonly ContentContractHandlerResolver _contractHandlerResolver;
    private readonly ContractCollection _contractCollection;
    private readonly StellarService _stellarService;
    private readonly CliClient _cliClient;

    public ContractService(SocketRequesterContext socketRequesterContext, LockManagerService lockManagerService, ContentService contentService, ContractCollection contractCollection, StellarService stellarService, ContentContractHandlerResolver contractHandlerResolver, CliClient cliClient)
    {
        _lockManagerService = lockManagerService;
        _contentService = contentService;
        _contractCollection = contractCollection;
        _stellarService = stellarService;
        _contractHandlerResolver = contractHandlerResolver;
        _cliClient = cliClient;
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
                    GlobalCache.Remove(nameof(FetchFederationContentLocal));
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

                foreach (var model in await FetchFederationContentForContracts())
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

    private async Task<IEnumerable<ContentContractsModel>> FetchFederationContentForContracts()
    {
        var federationContent = await FetchFederationContentLocal();
        var currencies = federationContent.Where(item => item is CurrencyContent)
            .Select(item => new ContentContractsModel(item));

        var itemsByType = federationContent
            .Where(item => item is ItemContent)
            .GroupBy(item =>
            {
                var idParts = item.Id.Split('.');
                return string.Join(".", idParts.Take(idParts.Length - 1));
            })
            .Select(g => new ContentContractsModel(g.First()));

        return currencies.Concat(itemsByType);
    }

    private async Task<List<IContentObject>> FetchFederationContentLocal()
    {
        return await GlobalCache.GetOrCreate<List<IContentObject>>(nameof(FetchFederationContentLocal), async _ =>
        {
            var federatedTypes = StellarFederationCommonHelper.GetFederationTypes();
            var manifest = await _contentService.GetManifest(new ContentQuery
            {
                TypeConstraints = federatedTypes
            });

            var result = new List<IContentObject>();
            foreach (var clientContentInfo in manifest.entries.Where(
                         item => item.contentId.StartsWith("currency") ||
                                 item.contentId.StartsWith("items")))
            {
                var contentRef = await clientContentInfo.Resolve();
                result.Add(contentRef);
            }
            return result;
        }, TimeSpan.FromDays(1)) ?? [];
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