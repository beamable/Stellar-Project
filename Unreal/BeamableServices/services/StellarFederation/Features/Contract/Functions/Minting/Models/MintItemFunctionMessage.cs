using System.Linq;
using MongoDB.Bson;
using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;

public record MintItemFunctionMessage(ObjectId[] TransactionIds, string ContentId, string ConcurrencyKey, string[] To, uint[] TokenIds, string[] MetadataHashes) : IFunctionMessage
{
    public string FunctionName => "batch_mint";

    public SCVal[] ToArgs() =>
    [
        new SCVec(
            To.Zip(TokenIds, (to, id) => (to, id))
                .Zip(MetadataHashes, (pair, hash) =>
                    new SCVec([
                        new ScAccountId(pair.to),
                        new SCUint32(pair.id),
                        new SCString(hash)
                    ])
                ).ToArray<SCVal>()
        )
    ];
}