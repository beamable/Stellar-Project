using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;

namespace Beamable.StellarFederation.Features.TransactionProcessor;

public interface ITransactionHandler
{
    Task HandleAsync(List<QueuedTransactionBase> transactions);
}