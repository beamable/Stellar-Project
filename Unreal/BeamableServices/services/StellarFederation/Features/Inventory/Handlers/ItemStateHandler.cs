using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.Inventory.Extensions;
using Beamable.StellarFederation.Features.Minting.Storage;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Features.Inventory.Handlers;

public class ItemStateHandler(
    ContractProxy contractProxy,
    MintCollection mintCollection
    ) : IService, IContentHandler
{
    public async Task<IFederatedState> GetState(string wallet, string contentId)
    {
        var contract = await contractProxy.GetContract<ItemContract>(contentId.ToContentType());
        var balance = await contractProxy.GetItemBalance(wallet, contentId.ToContentType());
        var mints = await mintCollection.GetTokenMints(contract.Address, balance);

        return new ItemsState
        {
            Items = mints.ToFederatedItems()
        };
    }
}