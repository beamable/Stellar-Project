using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Transactions.Storage.Models;

[BsonDiscriminator(nameof(ItemAddInventoryRequest))]
public class ItemAddInventoryRequest : QueuedTransactionBase
{
    public required string ContentId { get; init; }
    public required Dictionary<string, string> Properties { get; init; }
}