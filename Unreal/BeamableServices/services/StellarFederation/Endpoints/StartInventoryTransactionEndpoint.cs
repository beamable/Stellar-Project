using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Inventory;

namespace Beamable.StellarFederation.Endpoints;

public class StartInventoryTransactionEndpoint : IEndpoint
{
    public Task<FederatedInventoryProxyState> StartInventoryTransaction(string id, string transaction, Dictionary<string, long> currencies, List<FederatedItemCreateRequest> newItems, List<FederatedItemDeleteRequest> deleteItems, List<FederatedItemUpdateRequest> updateItems)
    {
        return Task.FromResult(new FederatedInventoryProxyState());
    }
}