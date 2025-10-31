using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Server;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Transactions.Exceptions;
using Beamable.StellarFederation.Features.Transactions.Storage;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using MongoDB.Bson;

namespace Beamable.StellarFederation.Features.Transactions;

public class TransactionManager
{
    private readonly AsyncLocal<ObjectId?> _currentTransaction = new();
    private static int _inflightTransactions;
    private readonly RequestContext _requestContext;
    private readonly InventoryTransactionCollection _inventoryTransactionCollection;
    private readonly TransactionLogCollection _transactionLogCollection;
    public static readonly string InstanceId = Guid.NewGuid().ToString();

    public TransactionManager(RequestContext requestContext, InventoryTransactionCollection inventoryTransactionCollection, TransactionLogCollection transactionLogCollection)
    {
        _requestContext = requestContext;
        _inventoryTransactionCollection = inventoryTransactionCollection;
        _transactionLogCollection = transactionLogCollection;
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        IncludeFields = true
    };
    public static void SetupShutdownHook()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            var inflightTransactions = _inflightTransactions;

            while (inflightTransactions > 0)
            {
                Thread.Sleep(500);
                inflightTransactions = _inflightTransactions;
            }
        };
    }

    public void SetCurrentTransactionContext(ObjectId transactionId)
    {
        _currentTransaction.Value = transactionId;
    }

    public async Task AddChainTransaction(ChainTransaction chainTransaction)
    {
        if (_currentTransaction.Value.HasValue)
            await _transactionLogCollection.AddChainTransaction(_currentTransaction.Value!.Value, chainTransaction);
    }

    public async Task<ObjectId> StartTransaction(long gamerTag, string walletAddress, string operationName, string inventoryTransaction, Dictionary<string, long> currencies, List<FederatedItemCreateRequest> newItems, List<FederatedItemDeleteRequest> deleteItems, List<FederatedItemUpdateRequest> updateItems)
    {
        return await StartTransaction(gamerTag, walletAddress, inventoryTransaction, operationName, new
        {
            currencies,
            newItems,
            deleteItems,
            updateItems
        });
    }

    public async Task<ObjectId> StartTransaction<TRequest>(long gamerTag, string walletAddress, string? inventoryTransaction, string operationName, TRequest request)
    {
        return await StartTransaction(gamerTag, walletAddress, inventoryTransaction, operationName, request, _requestContext.UserId, _requestContext.Path);
    }

    private async Task<ObjectId> StartTransaction<TRequest>(long gamerTag, string walletAddress, string? inventoryTransaction, string operationName, TRequest request, long requesterUserId, string path)
    {
        if (inventoryTransaction is not null)
        {
            var isSuccess = await _inventoryTransactionCollection.TryInsertInventoryTransaction(inventoryTransaction);
            if (!isSuccess) throw new TransactionException($"Inventory transaction {inventoryTransaction} already processed or in-progress");
        }

        var transactionId = ObjectId.GenerateNewId();
        await _transactionLogCollection.Insert(new TransactionLog
        {
            Id = transactionId,
            InventoryTransactionId = inventoryTransaction,
            RequesterUserId = requesterUserId,
            PlayerUserId = gamerTag,
            Wallet = walletAddress,
            Request = request as string ?? JsonSerializer.Serialize(request, JsonSerializerOptions),
            Path = path,
            OperationName = operationName
        });

        Interlocked.Increment(ref _inflightTransactions);

        return transactionId;
    }

    public async Task RunAsyncBlock(ObjectId transactionId, string? inventoryTransactionId, Func<Task> block)
    {
        try
        {
            await block();
            await TransactionDone(transactionId);
        }
        catch (Exception ex)
        {
            BeamableLogger.LogError("Error processing transaction {transaction}. Error: {e}", transactionId, ex.ToLogFormat());
            await TransactionError(transactionId, inventoryTransactionId, ex);
            throw;
        }
    }

    private async Task TransactionError(ObjectId transactionId, string? inventoryTransactionId, Exception ex)
    {
        Interlocked.Decrement(ref _inflightTransactions);
        if (inventoryTransactionId is not null)
        {
            BeamableLogger.Log("Clearing the inventory transaction {transactionId} record to enable retries.", inventoryTransactionId);
            await _inventoryTransactionCollection.DeleteInventoryTransaction(inventoryTransactionId);
        }
        await _transactionLogCollection.SetError(transactionId, ex.Message);
    }

    public async Task TransactionError(string transactionId, Exception ex)
    {
        Interlocked.Decrement(ref _inflightTransactions);
        var inventoryTransactionId = await _transactionLogCollection.GetInventoryTransaction(ObjectId.Parse(transactionId));
        if (!string.IsNullOrWhiteSpace(inventoryTransactionId))
        {
            BeamableLogger.Log("Clearing the inventory transaction {transactionId} record to enable retries.", inventoryTransactionId);
            await _inventoryTransactionCollection.DeleteInventoryTransaction(inventoryTransactionId);
        }
        await _transactionLogCollection.SetError(ObjectId.Parse(transactionId), ex.Message);
    }

    private async Task TransactionDone(ObjectId transactionId)
    {
        Interlocked.Decrement(ref _inflightTransactions);
        await _transactionLogCollection.SetDone(transactionId);
    }
}