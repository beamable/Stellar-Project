using System;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using StellarDotnetSdk.Xdr;

namespace Beamable.StellarFederation.Features.Contract;

public partial class ContractProxy
{
    public async Task<CoinBalanceResponse> GetCoinBalance(string wallet, string contentId)
    {
        var contract = await GetContract<ContractBase>(contentId);
        var balanceResponse = await _stellarRpcClient.InvokeContractAsync(contract.Address,
            new BalanceFunctionMessage(contentId,
                wallet));
        if (string.IsNullOrWhiteSpace(balanceResponse))
            return new CoinBalanceResponse(0);

        var reader = new XdrDataInputStream(Convert.FromBase64String(balanceResponse));
        var transactionResult = TransactionResult.Decode(reader);
        return new CoinBalanceResponse(transactionResult.Ext.Discriminant);
    }
}