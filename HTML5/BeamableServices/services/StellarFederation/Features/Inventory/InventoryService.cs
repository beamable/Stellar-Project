using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.Inventory.Storage;

namespace Beamable.StellarFederation.Features.Inventory;

public class InventoryService : IService
{
    private readonly InventoryStateCollection _inventoryStateCollection;

    public InventoryService(InventoryStateCollection inventoryStateCollection)
    {
        _inventoryStateCollection = inventoryStateCollection;
    }

    public async Task<FederatedInventoryProxyState> GetLastKnownState(string id)
    {
        var lastKnownState = await _inventoryStateCollection.Get(id.ToLower());
        return lastKnownState ?? new FederatedInventoryProxyState
        {
            currencies = new Dictionary<string, long>(),
            items = new Dictionary<string, List<FederatedItemProxy>>()
        };
    }
}