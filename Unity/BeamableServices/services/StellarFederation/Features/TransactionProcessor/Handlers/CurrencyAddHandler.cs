using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class CurrencyAddHandler(
    CoinCurrencyAddHandler coinCurrencyAddHandler,
    SupplyCurrencyAddHandler supplyCurrencyAddHandler) : IService, ITransactionHandler
{
    public async Task HandleAsync(List<QueuedTransactionBase> transactions)
    {
        var currencyAddInventoryRequest = transactions.First() as CurrencyAddInventoryRequest;
        switch (currencyAddInventoryRequest!.ContentId.ExtractMiddle())
        {
            case FederationContentTypes.RegularCoinType:
                await coinCurrencyAddHandler.HandleAsync(transactions);
                return;
            case FederationContentTypes.GoldCoinType:
                await supplyCurrencyAddHandler.HandleAsync(transactions);
                return;
            default:
                throw new NotSupportedException($"No handler found for content type: {currencyAddInventoryRequest!.ContentId}");
        }
    }
}