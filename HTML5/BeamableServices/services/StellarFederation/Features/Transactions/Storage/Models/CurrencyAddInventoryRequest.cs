using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Transactions.Storage.Models;

[BsonDiscriminator(nameof(CurrencyAddInventoryRequest))]
public class CurrencyAddInventoryRequest : QueuedTransactionBase
{
    public required string ContentId { get; init; }
    public required long Amount { get; init; }
}