using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Transactions.Storage.Models;

[BsonDiscriminator(nameof(AccountCreateRequest))]
public class AccountCreateRequest : QueuedTransactionBase
{

}