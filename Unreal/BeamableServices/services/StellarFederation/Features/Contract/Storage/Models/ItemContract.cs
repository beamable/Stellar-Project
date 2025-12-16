using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Contract.Storage.Models;

[BsonDiscriminator(nameof(ItemContract))]
public class ItemContract : ContractBase
{
    public required string BaseUri { get; init; }
}