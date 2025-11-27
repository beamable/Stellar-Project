using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common.Content;
using Beamable.StellarFederation.Features.Transactions.Models;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;

namespace Beamable.StellarFederation.Extensions;

public static class QueuedTransactionExtensions
{
    public static async Task<List<QueuedTransactionModel>> TransformWithKey(this List<QueuedTransactionBase> transactions,
        Func<string, Task<IContentObject>> fetchValueAsync)
    {
        var tasks = transactions.Select(async tx => new QueuedTransactionModel
        (
            tx,
            await fetchValueAsync(tx.ConcurrencyKey)
        ));
        return (await Task.WhenAll(tasks)).ToList();
    }

    public static async Task<List<QueuedTransactionModel>> TransformWithKeys(this List<QueuedTransactionBase> transactions,
        Func<IEnumerable<string>, Task<List<IContentObject>>> fetchValuesAsync)
    {
        var keys = transactions.Select(t => t.ConcurrencyKey);
        var valueMap = await fetchValuesAsync(keys);
        return transactions.Select(tx => new QueuedTransactionModel
        (
            tx,
            valueMap.First(v => v.Id == tx.ConcurrencyKey)
        )).ToList();
    }
}