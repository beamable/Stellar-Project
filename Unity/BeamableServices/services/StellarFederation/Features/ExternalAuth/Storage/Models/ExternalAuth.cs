using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.ExternalAuth.Storage.Models;

public record ExternalAuth(
    [property: BsonElement("_id")] string Message,
    long GamerTag,
    DateTime ExpiresAt
);