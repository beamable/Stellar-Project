using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.LockManager;
using Beamable.StellarFederation.Features.TransactionProcessor;
using Beamable.StellarFederation.Features.Transactions.Storage;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using MongoDB.Bson;

namespace Beamable.StellarFederation.Features.Transactions;

public partial class TransactionBatchService : IService
{
    private readonly TransactionManager _transactionManager;
    private readonly TransactionQueueCollection _transactionQueueCollection;
    private readonly LockManagerService _lockManagerService;
    private readonly TransactionProcessorService _processorService;

    public TransactionBatchService(TransactionQueueCollection transactionQueueCollection, LockManagerService lockManagerService, TransactionProcessorService processorService, TransactionManager transactionManager)
    {
        _transactionQueueCollection = transactionQueueCollection;
        _lockManagerService = lockManagerService;
        _processorService = processorService;
        _transactionManager = transactionManager;
    }

    public async Task<bool> AcquireLock(string lockName, int lockTimeoutSeconds = 5 * 60)
    {
        return await _lockManagerService.AcquireLock(lockName, lockTimeoutSeconds);
    }

    public async Task ReleaseLock(string lockName)
    {
        await _lockManagerService.ReleaseLock(lockName);
    }

    public async Task ReleaseLock(List<string> lockNames)
    {
        foreach (var lockName in lockNames)
        {
            await _lockManagerService.ReleaseLock(lockName);
        }
    }

    //Only if lock on concurrencyKey is acquired
    // public async Task<List<QueuedTransactionBase>> FetchBatch(string concurrencyKey, CancellationToken cancellationToken)
    // {
    //     return await _transactionQueueCollection.FetchBatch<QueuedTransactionBase>(concurrencyKey, cancellationToken);
    // }

    public async Task<List<QueuedTransactionBase>> FetchBatch(List<string> concurrencyKeys, CancellationToken cancellationToken)
    {
        return await _transactionQueueCollection.FetchBatch2<QueuedTransactionBase>(concurrencyKeys, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetPotentialWorkGroups(CancellationToken stoppingToken)
    {
        return await _transactionQueueCollection.GetPotentialWorkGroups(stoppingToken);
    }

    public async Task StartBatch(List<QueuedTransactionBase> transactions)
    {
        //var results = await transactions.TransformWithKeys(_contentService.GetContentObjects);
        await _processorService.Start(transactions);
    }

    public async Task UpdateStatus(ObjectId[] ids, TransactionStatus transactionStatus)
    {
        await _transactionQueueCollection.UpdateStatus(ids, transactionStatus);
    }
}