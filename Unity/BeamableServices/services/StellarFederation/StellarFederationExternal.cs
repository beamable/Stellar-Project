using System.Collections.Generic;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Server;
using Beamable.StellarFederation.Endpoints;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.ExternalAuth;
using StellarFederationCommon;

namespace Beamable.StellarFederation;

public partial class StellarFederation : IFederatedInventory<StellarWeb3ExternalIdentity>
{
    async Promise<FederatedAuthenticationResponse> IFederatedLogin<StellarWeb3ExternalIdentity>.Authenticate(string token, string challenge, string solution)
    {
        return await Provider.GetService<AuthenticateExternalEndpoint>()
            .Authenticate(token, challenge, solution);
    }
    Promise<FederatedInventoryProxyState> IFederatedInventory<StellarWeb3ExternalIdentity>.GetInventoryState(string id)
    {
        return new Promise<FederatedInventoryProxyState>();
    }

    async Promise<FederatedInventoryProxyState> IFederatedInventory<StellarWeb3ExternalIdentity>.StartInventoryTransaction(string id, string transaction, Dictionary<string, long> currencies, List<FederatedItemCreateRequest> newItems, List<FederatedItemDeleteRequest> deleteItems,
        List<FederatedItemUpdateRequest> updateItems)
    {
        var gamerTag = await Provider.GetService<AccountsService>().GetGamerTag(id);
        var microserviceInfo = MicroserviceMetadataExtensions.GetMetadata<StellarFederation, StellarWeb3ExternalIdentity>();
        return await Provider.GetService<StartInventoryTransactionEndpoint>()
            .StartInventoryTransaction(id, transaction, currencies, newItems, deleteItems, updateItems, gamerTag, microserviceInfo);
    }

    [Callable]
    public async Promise ExternalAddress()
    {
        await Provider.GetService<ExternalAuthService>().ProcessAddressCallback(Context.Body);
    }

    [Callable]
    public async Promise ExternalSignature()
    {
        await Provider.GetService<ExternalAuthService>().ProcessSignatureCallback(Context.Body);
    }
}