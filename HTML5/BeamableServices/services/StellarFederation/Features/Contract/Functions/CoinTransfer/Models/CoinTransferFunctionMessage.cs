using System.Linq;
using Beamable.StellarFederation.Features.Contract.Functions;
using MongoDB.Bson;
using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Models;

public record CoinTransferFunctionMessage(
    ObjectId[] TransactionIds,
    string ContentId,
    string ConcurrencyKey,
    string[] From,
    string[] To,
    long[] Amount) : IFunctionMessage
{
    public string FunctionName => "batch_transfer";

    public SCVal[] ToArgs() =>
    [
        new SCVec(
            From.Zip(To, (from, to) => (from, to))
                .Zip(Amount, (pair, amount) =>
                    new SCVec([
                        new ScAccountId(pair.from),
                        new ScAccountId(pair.to),
                        new SCInt128(amount.ToString())
                    ])
                ).ToArray<SCVal>()
        )
    ];
}