using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Functions.Approval.Models;

public record CoinAllowanceFunctionMessage(string ContentId, string Owner, string Spender) : IFunctionViewMessage
{
    public string FunctionName => "allowance";

    public SCVal[] ToArgs() =>
    [
        new ScAccountId(Owner),
        new ScAccountId(Spender)
    ];
}