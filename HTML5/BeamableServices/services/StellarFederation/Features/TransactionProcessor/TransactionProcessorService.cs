using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;

namespace Beamable.StellarFederation.Features.TransactionProcessor;

public class TransactionProcessorService(TransactionHandlerFactory handlerFactory) : IService
{
    public async Task Start(List<QueuedTransactionBase> transactions)
    {
        var tasks = transactions
            .GroupBy(t => t.GetType())
            .Select(async group =>
            {
                try
                {
                    var handler = handlerFactory.GetHandler(group.Key);
                    await handler.HandleAsync(group.ToList());
                }
                catch (Exception ex)
                {
                    BeamableLogger.LogError("Error processing {TransactionType} with error {ex}", group.Key.Name, ex.ToLogFormat());
                }
            });

        await Task.WhenAll(tasks);
    }
}