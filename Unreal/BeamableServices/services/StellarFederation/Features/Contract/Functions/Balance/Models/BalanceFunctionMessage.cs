using Beamable.StellarFederation.Features.Contract.Functions;
using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Models;

public record BalanceFunctionMessage(string ContentId, string Wallet) : IFunctionViewMessage
{
    public string FunctionName => "balance";

    public SCVal[] ToArgs() =>
    [
        new ScAccountId(Wallet)
    ];
}