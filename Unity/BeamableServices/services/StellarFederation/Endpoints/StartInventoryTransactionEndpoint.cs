using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Server;
using Beamable.StellarFederation.BackgroundService;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Transactions;
using Beamable.StellarFederation.Features.Transactions.Storage;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Endpoints;

public class StartInventoryTransactionEndpoint : IEndpoint
{
    private readonly TransactionBatchService _transactionBatchService;
    private readonly TransactionManager _transactionManager;
    private readonly RequestContext _requestContext;

    public StartInventoryTransactionEndpoint(RequestContext requestContext, TransactionBatchService transactionBatchService, TransactionManager transactionManager)
    {
        _requestContext = requestContext;
        _transactionBatchService = transactionBatchService;
        _transactionManager = transactionManager;
    }

    public async Task<FederatedInventoryProxyState> StartInventoryTransaction(string id, string transaction, Dictionary<string, long> currencies, List<FederatedItemCreateRequest> newItems, List<FederatedItemDeleteRequest> deleteItems, List<FederatedItemUpdateRequest> updateItems, long gamerTag,
        MicroserviceInfo microserviceInfo)
    {
        var transactionId = await _transactionManager.StartTransaction(gamerTag, id, nameof(StartInventoryTransaction), transaction, currencies, newItems, deleteItems, updateItems, _requestContext.UserId, _requestContext.Path);
        _ = _transactionManager.RunAsyncBlock(transactionId, transaction, async () =>
        {
            var currencyAddRequest = currencies
            .Where(c => c.Value > 0)
            .Select(c =>
                new CurrencyAddInventoryRequest
                {
                    Wallet = id,
                    GamerTag = gamerTag,
                    TransactionId = transactionId,
                    UniqueTransactionId = transaction,
                    ConcurrencyKey = c.Key.ToCurrencyModuleName(),
                    FunctionName = nameof(CurrencyAddInventoryRequest),
                    MicroserviceInfo = microserviceInfo,
                    Status = TransactionStatus.Pending,
                    ContentId = c.Key,
                    Amount = c.Value
                });
        await _transactionBatchService.Insert(currencyAddRequest);

        var currencySubtractRequest = currencies
            .Where(c => c.Value < 0)
            .Select(c =>
                new CurrencySubtractInventoryRequest
                {
                    Wallet = id,
                    GamerTag = gamerTag,
                    TransactionId = transactionId,
                    UniqueTransactionId = transaction,
                    ConcurrencyKey = c.Key.ToCurrencyModuleName(),
                    FunctionName = nameof(CurrencySubtractInventoryRequest),
                    MicroserviceInfo = microserviceInfo,
                    Status = TransactionStatus.Pending,
                    ContentId = c.Key,
                    Amount = Math.Abs(c.Value)
                });
        await _transactionBatchService.Insert(currencySubtractRequest);

        var itemAddRequest = newItems
            .Select(c =>
                new ItemAddInventoryRequest
                {
                    Wallet = id,
                    GamerTag = gamerTag,
                    TransactionId = transactionId,
                    UniqueTransactionId = transaction,
                    ConcurrencyKey = c.contentId.ToItemModuleName(),
                    FunctionName = nameof(ItemAddInventoryRequest),
                    MicroserviceInfo = microserviceInfo,
                    Status = TransactionStatus.Pending,
                    ContentId = c.contentId,
                    Properties = c.properties
                });
        await _transactionBatchService.Insert(itemAddRequest);

        BackgroundServiceState.ResetDelay();
        });

        return new FederatedInventoryProxyState();
    }
}