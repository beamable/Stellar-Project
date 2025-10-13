using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Accounts.Storage.Models;

public record Vault(
    [property: BsonElement("_id")] string Name,
    string Address,
    string SecretSeed,
    [property: BsonIgnoreIfNull] List<VaultBalance>? VaultBalance = null
);

public record VaultBalance(
    string Symbol,
    long Balance
);