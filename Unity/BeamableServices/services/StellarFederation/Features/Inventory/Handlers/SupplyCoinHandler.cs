using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract;

namespace Beamable.StellarFederation.Features.Inventory.Handlers;

public class SupplyCoinHandler(ContractProxy contractProxy) : IService, IContentHandler
{
    public async Task<IFederatedState> GetState(string wallet, string contentId)
    {
        var balance = await contractProxy.GetCoinBalance(wallet, contentId);
        return new CurrenciesState
        {
            Currencies = new Dictionary<string, long>
            {
                { contentId, balance.Amount }
            }
        };
    }
}