using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Contract.Storage.Models;

[BsonDiscriminator(nameof(CoinContract))]
public class CoinContract : ContractBase
{
}