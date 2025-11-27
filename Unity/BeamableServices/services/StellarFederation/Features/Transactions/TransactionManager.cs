using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Contract.Functions;
using Beamable.StellarFederation.Features.Transactions.Exceptions;
using Beamable.StellarFederation.Features.Transactions.Models;
using Beamable.StellarFederation.Features.Transactions.Storage;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using MongoDB.Bson;

namespace Beamable.StellarFederation.Features.Transactions;

public class TransactionManager : IService
{
    private static int _inflightTransactions;
    private readonly InventoryTransactionCollection _inventoryTransactionCollection;
    private readonly TransactionLogCollection _transactionLogCollection;
    private readonly TransactionQueueCollection _transactionQueueCollection;
    public static readonly string InstanceId = Guid.NewGuid().ToString();

    public TransactionManager(InventoryTransactionCollection inventoryTransactionCollection, TransactionLogCollection transactionLogCollection, TransactionQueueCollection transactionQueueCollection)
    {
        _inventoryTransactionCollection = inventoryTransactionCollection;
        _transactionLogCollection = transactionLogCollection;
        _transactionQueueCollection = transactionQueueCollection;
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

    // public void SetCurrentTransactionContext(ObjectId transactionId)
    // {
    //     _currentTransaction.Value = transactionId;
    // }
    //
    // public void SetCurrentTransactionContext(string transactionId)
    // {
    //     _currentTransaction.Value = ObjectId.Parse(transactionId);
    // }
    //
    // public async Task SetCurrentTransactionContextByInventoryTransaction(string inventoryTransactionId)
    // {
    //     var transactionLog = await _transactionLogCollection.GetByInventoryTransaction(inventoryTransactionId);
    //     if (transactionLog is not null)
    //     {
    //         _currentTransaction.Value = transactionLog.Id;
    //     }
    // }

    // public async Task AddChainTransaction(ChainTransaction chainTransaction)
    // {
    //     if (_currentTransaction.Value.HasValue)
    //         await _transactionLogCollection.AddChainTransaction(_currentTransaction.Value!.Value, chainTransaction);
    // }

    public async Task AddChainTransaction(ObjectId transactionId, ChainTransaction chainTransaction)
    {
        await _transactionLogCollection.AddChainTransaction(transactionId, chainTransaction);
    }

    public async Task AddChainTransactions<TFunctionNativeMessage>(TFunctionNativeMessage[] functionMessages, string transactionHash, string? error = null) where TFunctionNativeMessage : IFunctionNativeMessage
    {
        var tasks = functionMessages.Select(async x =>
                await _transactionLogCollection.AddChainTransaction(x.TransactionId, new ChainTransaction
                {
                    Data = JsonSerializer.Serialize(new
                    {
                        x.TransactionId,
                        x.FunctionName
                    }),
                    Function = x.FunctionName,
                    Hash = transactionHash,
                    Error = error
                })
        );
        await Task.WhenAll(tasks);
    }

    public async Task AddChainTransaction(string transactionId, ChainTransaction chainTransaction)
    {
        var transactionLog = await _transactionLogCollection.GetByInventoryTransaction(transactionId);
        if (transactionLog is not null)
            await _transactionLogCollection.AddChainTransaction(transactionLog.Id, chainTransaction);
    }

    public async Task<ObjectId> StartTransaction(long gamerTag, string walletAddress, string operationName, string inventoryTransaction, Dictionary<string, long> currencies, List<FederatedItemCreateRequest> newItems, List<FederatedItemDeleteRequest> deleteItems, List<FederatedItemUpdateRequest> updateItems, long requesterUserId, string path)
    {
        return await StartTransactionInternal(gamerTag, walletAddress, inventoryTransaction, operationName, new
        {
            currencies,
            newItems,
            deleteItems,
            updateItems
        }, requesterUserId, path);
    }

    public async Task<ObjectId> StartTransaction(NewCustomTransaction customTransaction)
    {
        return await StartTransactionInternal(customTransaction.GamerTag, customTransaction.WalletAddress,
            null, customTransaction.OperationName, customTransaction,
            customTransaction.GamerTag, customTransaction.OperationName);
    }

    private async Task<ObjectId> StartTransactionInternal<TRequest>(long gamerTag, string walletAddress, string? inventoryTransaction, string operationName, TRequest request, long requesterUserId, string path)
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

    // public async Task InsertTransactionInQueue(IEnumerable<QueuedTransactionBase> newItems)
    // {
    //     await _transactionQueueCollection.Insert(newItems.Select(i => new CurrencyAddInventoryRequest
    //     {
    //         Id = ObjectId.GenerateNewId(),
    //         MicroserviceInfo = i.MicroserviceInfo,
    //         Wallet = i.Wallet,
    //         GamerTag = i.GamerTag,
    //         TransactionId = "", //// CHECK!!!!!!!!!!!!!!!!!!!!!!!!!!
    //         ConcurrencyKey = i.ContentId.ContentIdToConcurrencyKey(),
    //         Status = TransactionStatus.Pending,
    //         ContentId = i.ContentId,
    //         Amount = i.Amount,
    //         Properties = i.Properties.ToDictionary()
    //     }));
    // }
}