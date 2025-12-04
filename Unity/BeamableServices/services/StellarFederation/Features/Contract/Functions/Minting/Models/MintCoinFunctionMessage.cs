using System.Linq;
using MongoDB.Bson;
using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;

public record MintCoinFunctionMessage(ObjectId[] TransactionIds, string ContentId, string ConcurrencyKey, string[] To, long[] Amount) : IFunctionMessage
{
    public string FunctionName => "batch_mint";

    public SCVal[] ToArgs() =>
    [
        new SCVec(
            To.Zip(Amount, (address, amount) =>
                new SCVec([
                    new ScAccountId(address),
                    new SCInt128(amount.ToString())
                ])
            ).ToArray<SCVal>()
        )
    ];
}