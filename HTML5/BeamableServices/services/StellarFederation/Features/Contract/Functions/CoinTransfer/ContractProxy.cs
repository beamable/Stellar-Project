using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;

namespace Beamable.StellarFederation.Features.Contract;

public partial class ContractProxy
{
    public async Task<string> CoinBatchTransfer(CoinTransferFunctionMessage request)
    {
        var contract = await GetContract<GoldContract>(request.ContentId);
        return await _stellarRpcClient.SendTransactionAsync(contract.Address, request);
    }
}