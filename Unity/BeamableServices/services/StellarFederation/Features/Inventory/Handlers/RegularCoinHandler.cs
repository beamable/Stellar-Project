using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract;

namespace Beamable.StellarFederation.Features.Inventory.Handlers;

public class RegularCoinHandler : IService, IContentHandler
{
    private readonly ContractProxy _contractProxy;

    public RegularCoinHandler(ContractProxy contractProxy)
    {
        _contractProxy = contractProxy;
    }

    public async Task<IFederatedState> GetState(string wallet, string contentId)
    {
        var balance = await _contractProxy.GetCoinBalance(wallet, contentId);
        return new CurrenciesState
        {
            Currencies = new Dictionary<string, long>
            {
                { contentId, balance.Amount }
            }
        };
    }
}