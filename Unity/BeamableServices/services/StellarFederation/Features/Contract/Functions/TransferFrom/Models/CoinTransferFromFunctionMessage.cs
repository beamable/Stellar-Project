using System.Linq;
using MongoDB.Bson;
using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Functions.TransferFrom.Models;

public record CoinTransferFromFunctionMessage(
    ObjectId TransactionId,
    string ContentId,
    string[] Spender,
    string[] From,
    string[] To,
    long[] Amount) : IFunctionMessage
{
    public string FunctionName => "batch_transfer_from";

    public SCVal[] ToArgs() =>
    [
        new SCVec(
            Spender.Zip(From, (spender, from) => (spender, from))
                .Zip(To, (pair, to) => (pair.spender, pair.from, to))
                .Zip(Amount, (triple, amount) =>
                    new SCVec([
                        new ScAccountId(triple.spender),
                        new ScAccountId(triple.from),
                        new ScAccountId(triple.to),
                        new SCInt128(amount.ToString())
                    ])
                ).ToArray<SCVal>()
        )
    ];
}