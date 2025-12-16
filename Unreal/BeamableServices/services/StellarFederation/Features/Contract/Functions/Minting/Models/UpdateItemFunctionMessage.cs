using System.Linq;
using MongoDB.Bson;
using StellarDotnetSdk.Soroban;

namespace Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;

public record UpdateItemFunctionMessage(ObjectId[] TransactionIds, string ContentId, string ConcurrencyKey, uint[] TokenIds, string[] MetadataHashes) : IFunctionMessage
{
    public string FunctionName => "batch_update_metadata";

    public SCVal[] ToArgs() =>
    [
        new SCVec(
            TokenIds.Zip(MetadataHashes, (id, hash) =>
                new SCVec([
                    new SCUint32(id),
                    new SCString(hash)
                ])
            ).ToArray<SCVal>()
        )
    ];
}