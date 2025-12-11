using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;

public record ItemBalanceFunctionMessage(string ContentId, string Wallet) : IFunctionViewMessage
{
    public string FunctionName => "get_wallet_tokens";

    public SCVal[] ToArgs() =>
    [
        new ScAccountId(Wallet)
    ];
}