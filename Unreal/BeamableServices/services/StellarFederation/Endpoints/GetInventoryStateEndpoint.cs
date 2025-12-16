using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Content;
using Beamable.StellarFederation.Features.Inventory;
using Beamable.StellarFederation.Features.Inventory.Handlers;
using Beamable.StellarFederation.Features.Inventory.Storage;
using Beamable.StellarFederation.Features.Inventory.Storage.Models;

namespace Beamable.StellarFederation.Endpoints;

public class GetInventoryStateEndpoint : IEndpoint
{
    private readonly BeamContentService _beamContentService;
    private readonly ContentHandlerFactory _contentHandlerFactory;
    private readonly InventoryStateCollection _inventoryStateCollection;
    public GetInventoryStateEndpoint(BeamContentService beamContentService, ContentHandlerFactory contentHandlerFactory, InventoryStateCollection inventoryStateCollection)
    {
        _beamContentService = beamContentService;
        _contentHandlerFactory = contentHandlerFactory;
        _inventoryStateCollection = inventoryStateCollection;
    }

    public async Task<FederatedInventoryProxyState> GetInventoryState(string wallet, MicroserviceInfo microserviceInfo)
    {
        var resultState = new FederatedInventoryProxyState
        {
            currencies = new Dictionary<string, long>(),
            items = new Dictionary<string, List<FederatedItemProxy>>(),
        };

        foreach (var contentObject in await _beamContentService.FetchFederationContentForState(microserviceInfo))
        {
            var handler = _contentHandlerFactory.GetHandler(contentObject);
            if (handler is null) continue;
            var state = await handler.GetState(wallet, contentObject.Id);
            switch (state)
            {
                case CurrenciesState currencyState:
                {
                    foreach (var kvp in currencyState.Currencies)
                    {
                        resultState.currencies.GetOrAdd(kvp.Key, kvp.Value);
                    }
                    break;
                }
                case ItemsState itemsState:
                {
                    foreach (var kvp in itemsState.Items)
                    {
                        if (!resultState.items.ContainsKey(kvp.Key))
                            resultState.items[kvp.Key] = [];

                        resultState.items[kvp.Key].AddRange(kvp.Value);
                    }

                    break;
                }
            }
        }

        await _inventoryStateCollection.Save(new InventoryState
        {
            Id = wallet.ToLower(),
            Inventory = resultState
        });

        return resultState;
    }
}