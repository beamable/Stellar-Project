using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.ExternalAuth.Storage.Models;

public record ExternalAuth(
    [property: BsonElement("_id")] string Address,
    long GamerTag,
    string? Message,
    DateTime ExpiresAt
);