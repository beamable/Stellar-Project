using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Contract.Storage.Models;

[BsonDiscriminator(nameof(GoldContract))]
public class GoldContract : ContractBase
{
    public required string TotalSupply { get; init; }
}