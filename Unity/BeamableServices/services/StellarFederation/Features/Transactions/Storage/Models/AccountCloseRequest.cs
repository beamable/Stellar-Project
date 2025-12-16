using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Transactions.Storage.Models;

[BsonDiscriminator(nameof(AccountCloseRequest))]
public class AccountCloseRequest : QueuedTransactionBase
{

}