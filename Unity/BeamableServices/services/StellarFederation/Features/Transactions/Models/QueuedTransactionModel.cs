using Beamable.Common.Content;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using MongoDB.Bson;

namespace Beamable.StellarFederation.Features.Transactions.Models;

public readonly struct QueuedTransactionModel(QueuedTransactionBase Base, IContentObject Content);