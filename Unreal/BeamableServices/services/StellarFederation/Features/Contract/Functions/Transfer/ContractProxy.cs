using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract.Functions.TransferFrom.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;

namespace Beamable.StellarFederation.Features.Contract;

public partial class ContractProxy
{
    public async Task<string> CoinTransferFromUser(CoinTransferFromUserFunctionMessage message)
    {
        var contract = await GetContract<ContractBase>(message.ContentId);
        return await _stellarRpcClient.SendDecoupledTransactionAsync(contract.Address, message);
    }
}