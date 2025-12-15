using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract.Functions.TransferFrom.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;

namespace Beamable.StellarFederation.Features.Contract;

public partial class ContractProxy
{
    public async Task<string> TransferFrom(CoinTransferFromFunctionMessage coinTransferFromFunctionMessage)
    {
        var contract = await GetContract<ContractBase>(coinTransferFromFunctionMessage.ContentId);
        return await _stellarRpcClient.SendTransactionAsync(contract.Address, coinTransferFromFunctionMessage);
    }
}