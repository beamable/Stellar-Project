using System;
using Beamable.StellarFederation.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.StellarFederation.Features.Transactions.Storage.Models;

public class QueuedTransactionBase
{
    [BsonId] public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();
    public required string Wallet { get; set; }
    public required long GamerTag { get; set; }
    public required ObjectId TransactionId { get; set; }
    public required string UniqueTransactionId { get; set; }
    public required string ConcurrencyKey { get; init; } //This will be the contentId, type part for items, full name for else (because contracts are created with that logic)
    public required string FunctionName { get; init; }
    public required MicroserviceInfo MicroserviceInfo { get; set; }

    [property: BsonRepresentation(BsonType.String)]
    public required TransactionStatus Status { get; set; }
    public DateTime Created { get; init; } = DateTime.UtcNow;
    public DateTime? LockedAt { get; set; }
    public DateTime Expiration { get; set; } = DateTime.UtcNow.AddMinutes(5);
    public string? ProcessingInstanceId { get; set; }
    public string? ErrorMessage { get; set; }
}

public enum TransactionStatus { Pending, Processing, Sent, Completed, Retrying, Failed }