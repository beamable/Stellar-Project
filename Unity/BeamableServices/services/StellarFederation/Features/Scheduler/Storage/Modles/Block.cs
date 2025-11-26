using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Scheduler.Storage.Modles;

public class Block
{
    [BsonElement("_id")]
    public string Network { get; set; } = null!;
    public long BlockNumber { get; set; }
}