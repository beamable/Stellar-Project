using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Server;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Beamable.StellarFederation.Features.Transactions.Storage;

public class TransactionQueueCollection(IStorageObjectConnectionProvider storageObjectConnectionProvider, Configuration configuration)
    : IService
{
    private readonly Promise<IMongoDatabase> _databaseTask = storageObjectConnectionProvider.StellarFederationStorageDatabase();
    private readonly ConcurrentDictionary<Type, object> _collections = new();
    private readonly ConcurrentDictionary<Type, Task> _indexCreationTasks = new();

    private async ValueTask<IMongoCollection<TTransaction>> Get<TTransaction>() where TTransaction : QueuedTransactionBase
    {
        var database = await _databaseTask;
        var collection = (IMongoCollection<TTransaction>)_collections.GetOrAdd(
            typeof(TTransaction), _ => database.GetCollection<TTransaction>("transaction-queue")
        );
        await EnsureIndexesAsync(collection);
        return collection;
    }

    private async ValueTask EnsureIndexesAsync<TTransaction>(IMongoCollection<TTransaction> collection) where TTransaction : QueuedTransactionBase
    {
        if (!_indexCreationTasks.ContainsKey(typeof(TTransaction)))
        {
            _indexCreationTasks[typeof(TTransaction)] = Task.Run(async () =>
            {
                await collection.Indexes.CreateManyAsync([
                    new CreateIndexModel<TTransaction>(Builders<TTransaction>.IndexKeys.Ascending(x => x.Expiration),
                        new CreateIndexOptions
                        {
                            Name = "expiry",
                            ExpireAfter = TimeSpan.Zero
                        }),
                    new CreateIndexModel<TTransaction>(Builders<TTransaction>.IndexKeys
                        .Ascending(x => x.TransactionId)),
                    new CreateIndexModel<TTransaction>(Builders<TTransaction>.IndexKeys
                        .Ascending(x => x.Status)
                        .Ascending(x => x.Created)),
                    new CreateIndexModel<TTransaction>(Builders<TTransaction>.IndexKeys
                        .Ascending(x => x.Status)
                        .Ascending(x => x.ConcurrencyKey)
                        .Ascending(x => x.Created))
                ]);
            });
        }

        await _indexCreationTasks[typeof(TTransaction)];
    }

    public async Task<bool> Insert<TTransaction>(TTransaction transaction) where TTransaction : QueuedTransactionBase
    {
        var collection = await Get<TTransaction>();
        try
        {
            await collection.InsertOneAsync(transaction);
            return true;
        }
        catch (MongoWriteException)
        {
            return false;
        }
    }

    public async Task<bool> Insert<TTransaction>(IEnumerable<TTransaction> transactions) where TTransaction : QueuedTransactionBase
    {
        var collection = await Get<TTransaction>();
        try
        {
            var inserts = transactions.ToList();
            if (inserts.Count > 0)
            {
                await collection.InsertManyAsync(inserts, new InsertManyOptions
                {
                    IsOrdered = false
                });
            }
            return true;
        }
        catch (MongoWriteException)
        {
            return false;
        }
    }

    // public async Task<IEnumerable<string>> GetPotentialWorkGroupsOld(CancellationToken cancellationToken)
    // {
    //     var collection = await Get<QueuedTransactionBase>();
    //     var filter = Builders<QueuedTransactionBase>.Filter.In(x => x.Status, TransactionProcessValidStatus);
    //     var cursor = await collection.DistinctAsync(tx => tx.ConcurrencyKey, filter, null, cancellationToken);
    //     var results = await cursor.ToListAsync(cancellationToken);
    //     return results?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(s => s!) ?? [];
    // }

    public async Task<IEnumerable<string>> GetPotentialWorkGroups(CancellationToken cancellationToken)
    {
        var collection = await Get<QueuedTransactionBase>();
        // Get all keys currently being processed
        var processingFilter = Builders<QueuedTransactionBase>.Filter.Eq(x => x.Status, TransactionStatus.Processing);
        var processingKeysCursor = await collection.DistinctAsync(tx => tx.ConcurrencyKey, processingFilter, null, cancellationToken);
        var processingKeys = (await processingKeysCursor.ToListAsync(cancellationToken))?.ToHashSet() ?? [];

        // Get keys with valid statuses (Pending/Retrying)
        var validFilter = Builders<QueuedTransactionBase>.Filter.In(x => x.Status, TransactionProcessValidStatus);
        var validKeysCursor = await collection.DistinctAsync(tx => tx.ConcurrencyKey, validFilter, null, cancellationToken);
        var validKeys = await validKeysCursor.ToListAsync(cancellationToken);

        // Return valid keys that aren't currently processing
        return validKeys?
            .Where(x => !string.IsNullOrWhiteSpace(x) && !processingKeys.Contains(x))
            .Select(s => s!) ?? [];
    }

    public async Task<List<TTransaction>> FetchBatch<TTransaction>(string concurrencyKey, CancellationToken cancellationToken) where TTransaction : QueuedTransactionBase
    {
        var collection = await Get<TTransaction>();
        try
        {
            var maxBatchSize = await configuration.MessageQueueBatchLimit;
            var batchFilter = Builders<TTransaction>.Filter.And(
                Builders<TTransaction>.Filter.In(tx => tx.Status, TransactionProcessValidStatus),
                Builders<TTransaction>.Filter.Eq(tx => tx.ConcurrencyKey, concurrencyKey)
            );
            var sort = Builders<TTransaction>.Sort.Ascending(tx => tx.Created);
            var candidates = await collection
                .Find(batchFilter)
                .Sort(sort)
                .Limit(maxBatchSize)
                .ToListAsync(cancellationToken);
            if (candidates.Count == 0)
            {
                BeamableLogger.LogWarning("Acquired lock for key {Key}, but found no pending documents.", concurrencyKey);
                return [];
            }

            // Group by ConcurrencyKey, then select first function for each contract
            var batch = candidates
                .GroupBy(x => x.ConcurrencyKey)
                .SelectMany(contractGroup =>
                {
                    // For each contract, pick the first function (FIFO)
                    var firstFunction = contractGroup.First().FunctionName;

                    // Return all operations for that contract-function combo
                    return contractGroup
                        .Where(op => op.FunctionName == firstFunction)
                        .Take(maxBatchSize);
                })
                .Take(maxBatchSize)
                .ToList();

            // CLAIM THE INDIVIDUAL DOCUMENTS IN THE BATCH
            var now = DateTime.UtcNow;
            var claimFilter = Builders<TTransaction>.Filter.In(tx => tx.Id, batch.Select(tx => tx.Id));
            var claimUpdate = Builders<TTransaction>.Update
                .Set(tx => tx.Status, TransactionStatus.Processing)
                .Set(tx => tx.LockedAt, now)
                .Set(tx => tx.ProcessingInstanceId, TransactionManager.InstanceId);
            await collection.UpdateManyAsync(claimFilter, claimUpdate, null, cancellationToken);
            foreach (var tx in batch)
            {
                tx.Status = TransactionStatus.Processing;
                tx.LockedAt = now;
                tx.ProcessingInstanceId = TransactionManager.InstanceId;
            }
            return batch;
        }
        catch (MongoException)
        {
            return [];
        }
    }

    public async Task<List<TTransaction>> FetchBatch2<TTransaction>(List<string> concurrencyKeys, CancellationToken cancellationToken) where TTransaction : QueuedTransactionBase
    {
        var collection = await Get<TTransaction>();
        try
        {
            var maxBatchSize = await configuration.MessageQueueBatchLimit;

            var batchFilter = Builders<TTransaction>.Filter.And(
                Builders<TTransaction>.Filter.In(tx => tx.Status, TransactionProcessValidStatus),
                Builders<TTransaction>.Filter.In(tx => tx.ConcurrencyKey, concurrencyKeys));
            var sort = Builders<TTransaction>.Sort.Ascending(tx => tx.Created);
            var candidates = await collection
                .Find(batchFilter)
                .Sort(sort)
                .Limit(maxBatchSize)
                .ToListAsync(cancellationToken);
            if (candidates.Count == 0)
                return [];

            var desiredBatch = new List<TTransaction>();
            var contractGroups = candidates.GroupBy(x => x.ConcurrencyKey);
            foreach (var contractGroup in contractGroups)
            {
                var firstFunction = contractGroup.First().FunctionName;
                var batchPerFunction = await GetMaxBatchLimit(firstFunction);
                var contractBatch = contractGroup
                    .Where(op => op.FunctionName == firstFunction)
                    .Take(batchPerFunction - desiredBatch.Count)
                    .ToList();

                desiredBatch.AddRange(contractBatch);

                if (desiredBatch.Count >= batchPerFunction)
                    break;
            }
            if (desiredBatch.Count == 0)
                return [];

            var ids = desiredBatch.Select(o => o.Id).ToList();
            var now = DateTime.UtcNow;

            var filter = Builders<TTransaction>.Filter.And(
                Builders<TTransaction>.Filter.In(x => x.Id, ids),
                Builders<TTransaction>.Filter.In(tx => tx.Status, TransactionProcessValidStatus)
            );
            var update = Builders<TTransaction>.Update
                .Set(x => x.Status, TransactionStatus.Processing)
                .Set(x => x.LockedAt, now)
                .Set(x => x.ProcessingInstanceId, TransactionManager.InstanceId);
            await collection.UpdateManyAsync(
                filter,
                update,
                cancellationToken: cancellationToken);

            var lockedOps = await collection
                .Find(x => ids.Contains(x.Id) &&
                           x.Status == TransactionStatus.Processing &&
                           x.ProcessingInstanceId == TransactionManager.InstanceId &&
                           x.LockedAt == now)
                .ToListAsync(cancellationToken);

            return lockedOps;
        }
        catch (MongoException)
        {
            return [];
        }
    }

    public async Task UpdateStatus(ObjectId[] transactionIds, TransactionStatus transactionStatus)
    {
        var collection = await Get<QueuedTransactionBase>();
        try
        {
            var filter = Builders<QueuedTransactionBase>.Filter.And(
                Builders<QueuedTransactionBase>.Filter.In(x => x.TransactionId, transactionIds)
            );
            var update = Builders<QueuedTransactionBase>.Update
                .Set(x => x.Status, transactionStatus)
                .Set(x => x.ProcessingInstanceId, TransactionManager.InstanceId);
            await collection.UpdateManyAsync(
                filter,
                update);
        }
        catch (MongoException)
        {
        }
    }

    public async Task Delete(IEnumerable<ObjectId> transactionIds)
    {
        var collection = await Get<QueuedTransactionBase>();
        try
        {
            var filter = Builders<QueuedTransactionBase>.Filter.And(
                Builders<QueuedTransactionBase>.Filter.In(x => x.TransactionId, transactionIds)
            );
            await collection.DeleteManyAsync(filter);
        }
        catch (MongoException)
        {
        }
    }

    private async ValueTask<int> GetMaxBatchLimit(string functionName)
    {
        if (functionName == nameof(AccountCreateRequest))
            return await configuration.MessageQueueBatchLimitNative;
        return await configuration.MessageQueueBatchLimit;
    }

    private static TransactionStatus[] TransactionProcessValidStatus => [TransactionStatus.Pending, TransactionStatus.Retrying];
}