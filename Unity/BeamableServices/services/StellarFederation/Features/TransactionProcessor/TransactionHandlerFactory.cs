using System;
using Beamable.Common.Dependencies;
using Beamable.StellarFederation.Features.TransactionProcessor.Handlers;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Beamable.StellarFederation.Features.TransactionProcessor;

public class TransactionHandlerFactory(IDependencyProvider serviceProvider) : IService
{
    public ITransactionHandler GetHandler(Type transactionType)
    {
        return transactionType.Name switch
        {
            nameof(AccountCreateRequest) => serviceProvider.GetRequiredService<AccountCreateHandler>(),
            nameof(CurrencyAddInventoryRequest) => serviceProvider.GetRequiredService<CurrencyAddHandler>(),
            nameof(ItemAddInventoryRequest) => serviceProvider.GetRequiredService<NftCreateHandler>(),
            nameof(ItemUpdateInventoryRequest) => serviceProvider.GetRequiredService<NftUpdateHandler>(),
            nameof(CurrencySubtractInventoryRequest) => serviceProvider.GetRequiredService<CurrencySubtractHandler>(),
            _ => throw new NotSupportedException($"No handler found for transaction type: {transactionType.Name}")
        };
    }
}