using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Accounts.Storage.Models;

public record Sequence(
    [property: BsonElement("_id")] string Address,
    long State,
    HashSet<long> Errors
);