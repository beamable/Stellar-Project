using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Minting.Storage.Models;

public class Counter
{
    [BsonElement("_id")]
    public string Name { get; set; } = null!;

    public uint State { get; set; }
}