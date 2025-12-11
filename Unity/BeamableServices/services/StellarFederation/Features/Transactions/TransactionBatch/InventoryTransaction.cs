using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;

namespace Beamable.StellarFederation.Features.Transactions;

public partial class TransactionBatchService
{
    public async Task Insert(IEnumerable<CurrencyAddInventoryRequest> currencyAddRequest)
    {
        await _transactionQueueCollection.Insert(currencyAddRequest);
    }

    public async Task Insert(IEnumerable<CurrencySubtractInventoryRequest> currencySubtractRequest)
    {
        await _transactionQueueCollection.Insert(currencySubtractRequest);
    }

    public async Task Insert(IEnumerable<ItemAddInventoryRequest> itemAddRequest)
    {
        await _transactionQueueCollection.Insert(itemAddRequest);
    }
    public async Task Insert(IEnumerable<ItemUpdateInventoryRequest> itemUpdateRequest)
    {
        await _transactionQueueCollection.Insert(itemUpdateRequest);
    }
}