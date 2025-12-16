using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class CurrencySubtractHandler(
    CoinCurrencySubtractHandler coinCurrencySubtractHandler) : IService, ITransactionHandler
{
    public async Task HandleAsync(List<QueuedTransactionBase> transactions)
    {
        await coinCurrencySubtractHandler.HandleAsync(transactions);
    }
}