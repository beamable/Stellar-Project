using System.Threading.Tasks;
using Beamable.Common;

namespace Beamable.StellarFederation.Endpoints;

public class GetInventoryStateEndpoint : IEndpoint
{
    public Task<FederatedInventoryProxyState> GetInventoryState(string wallet)
    {
        return Task.FromResult(new FederatedInventoryProxyState());
    }
}