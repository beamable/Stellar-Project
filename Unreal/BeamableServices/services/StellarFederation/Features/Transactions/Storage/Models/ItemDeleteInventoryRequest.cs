using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Transactions.Storage.Models;

[BsonDiscriminator(nameof(ItemDeleteInventoryRequest))]
public class ItemDeleteInventoryRequest : QueuedTransactionBase
{
    public required string ContentId { get; init; }
    public required string ProxyId { get; init; }
}