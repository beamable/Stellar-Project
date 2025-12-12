using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract.Functions.TransferFrom.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;

namespace Beamable.StellarFederation.Features.Contract;

public partial class ContractProxy
{
    public async Task TransferFrom(string owner, string contentId)
    {
        var contract = await GetContract<ContractBase>(contentId);
        //var message = new CoinTransferFromFunctionMessage()
    }
}