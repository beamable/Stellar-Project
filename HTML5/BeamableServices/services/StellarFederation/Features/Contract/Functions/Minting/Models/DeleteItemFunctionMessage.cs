using System.Linq;
using MongoDB.Bson;
using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;

public record DeleteItemFunctionMessage(
    ObjectId[] TransactionIds,
    string ContentId,
    string ConcurrencyKey,
    long[] GamerTags,
    string[] From,
    uint[] TokenIds,
    uint[] ExpirationLedger) : IFunctionMessageDecouple
{
    public string FunctionName => "batch_burn";

    public SCVal[] ToArgs() =>
    [
        new SCVec(
            From.Zip(TokenIds, (address, id) =>
                new SCVec([
                    new ScAccountId(address),
                    new SCUint32(id)
                ])
            ).ToArray<SCVal>()
        )
    ];
}