using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;

namespace Beamable.StellarFederation.Features.Contract;

public partial class ContractProxy
{
    public async Task<string> CoinBatchMint(MintCoinFunctionMessage request)
    {
        var contract = await GetContract<CoinContract>(request.ContentId);
        return await _stellarRpcClient.SendTransactionAsync(contract.Address, request);
    }
}