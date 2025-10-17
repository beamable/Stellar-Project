using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Contract.Storage.Models;

public class ContractBase
{
    [BsonId]
    public required string ContentId { get; init; }
    public required string Address { get; init; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}