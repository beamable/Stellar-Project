using System.Linq;
using MongoDB.Bson;
using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Functions.TransferFrom.Models;

public record CoinTransferFromUserFunctionMessage(
    ObjectId[] TransactionIds,
    string ContentId,
    string ConcurrencyKey,
    long[] GamerTags,
    string[] From,
    string[] To,
    long[] Amount,
    uint[] ExpirationLedger) : IFunctionMessageDecouple
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

public record CoinTransferFunctionMessageSingle(
    ObjectId TransactionIds,
    string ContentId,
    string ConcurrencyKey,
    string Spender,
    string From,
    string To,
    long Amount);