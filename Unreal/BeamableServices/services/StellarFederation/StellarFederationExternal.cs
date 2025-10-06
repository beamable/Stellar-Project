using System.Collections.Generic;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Server;
using Beamable.StellarFederation.Endpoints;
using StellarFederationCommon;

namespace Beamable.StellarFederation;

public partial class StellarFederation : IFederatedInventory<StellarWeb3ExternalIdentity>
{
    async Promise<FederatedAuthenticationResponse> IFederatedLogin<StellarWeb3ExternalIdentity>.Authenticate(string token, string challenge, string solution)
    {
        return await Provider.GetService<AuthenticateExternalEndpoint>()
            .Authenticate(token, challenge, solution);
    }

    async Promise<FederatedInventoryProxyState> IFederatedInventory<StellarWeb3ExternalIdentity>.GetInventoryState(string id)
    {
        return await Provider.GetService<GetInventoryStateEndpoint>()
            .GetInventoryState(id);
    }

    Promise<FederatedInventoryProxyState> IFederatedInventory<StellarWeb3ExternalIdentity>.StartInventoryTransaction(string id, string transaction, Dictionary<string, long> currencies, List<FederatedItemCreateRequest> newItems, List<FederatedItemDeleteRequest> deleteItems,
        List<FederatedItemUpdateRequest> updateItems)
    {
        return new Promise<FederatedInventoryProxyState>();
    }
}