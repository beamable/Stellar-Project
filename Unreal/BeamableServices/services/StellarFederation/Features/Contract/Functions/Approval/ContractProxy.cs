using System;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract.Functions.Approval.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.Stellar;
using Beamable.StellarFederation.Features.Stellar.Extensions;
using StellarDotnetSdk.Xdr;

namespace Beamable.StellarFederation.Features.Contract;

public partial class ContractProxy
{
    private const long MaxApproval = 10_000_000_000;

    public async Task<CoinAllowanceResponse> GetCoinAllowance(string owner, string contentId)
    {
        var contract = await GetContract<ContractBase>(contentId);
        var ownerAccount = await _accountsService.GetAccount(contentId);
        var balanceResponse = await _stellarRpcClient.InvokeContractAsync(contract.Address,
            new CoinAllowanceFunctionMessage(contentId,
                owner, ownerAccount!.Value.Address));
        if (string.IsNullOrWhiteSpace(balanceResponse))
            return new CoinAllowanceResponse(0);

        var reader = new XdrDataInputStream(Convert.FromBase64String(balanceResponse));
        var scVal = SCVal.Decode(reader);
        var amount = scVal.ToInt64OrZero();
        return new CoinAllowanceResponse(amount);
    }

    public async Task<string> CoinApproval(CoinApprovalFunctionRequest request)
    {
        var contract = await GetContract<ContractBase>(request.ContentId);
        var contractAccount = await _accountsService.GetAccount(request.ContentId);
        var latestLedger = await _stellarService.GetCurrentLedgerSequence();
        var liveUntil = StellarServiceExtensions.ExpiresInDays(latestLedger);
        var message = new CoinApprovalFunctionMessage(
            request.TransactionIds,
            request.ContentId,
            request.ConcurrencyKey,
            request.GamerTags,
            request.From,
            Enumerable.Repeat(contractAccount!.Value.Address, request.Amount.Length).ToArray(),
            Enumerable.Repeat(MaxApproval, request.Amount.Length).ToArray(),
            Enumerable.Repeat(liveUntil, request.Amount.Length).ToArray());
        return await _stellarRpcClient.SendDecoupledTransactionAsync(contract.Address, message);
    }
}