using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Inventory.Storage.Models;

public class Mint
{
    [BsonId]
    public ObjectId Id { get; set; } = new();
    public string ContentId { get; set; } = null!;
    public string ContractId { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public string InitialOwnerAddress { get; set; } = null!;
    public string[] ObjectIds { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = new();

    public DateTime Minted { get; set; } = DateTime.UtcNow;
}