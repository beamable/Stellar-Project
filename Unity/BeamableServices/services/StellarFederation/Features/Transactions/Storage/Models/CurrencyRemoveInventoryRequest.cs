using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Transactions.Storage.Models;

[BsonDiscriminator(nameof(CurrencySubtractInventoryRequest))]
public class CurrencySubtractInventoryRequest : QueuedTransactionBase
{
    public required string ContentId { get; init; }
    public required long Amount { get; init; }
}