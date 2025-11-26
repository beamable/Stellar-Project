using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Transactions;
using Microsoft.Extensions.Logging;

namespace Beamable.StellarFederation.BackgroundService;

public class StellarBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly ILogger<StellarBackgroundService> _logger;
    private readonly BackoffDelayManager _delayManager;
    private TransactionBatchService? _batchService;

    public StellarBackgroundService(ILogger<StellarBackgroundService> logger, BackoffDelayManager delayManager)
    {
        _logger = logger;
        _delayManager = delayManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_batchService is null)
                {
                    AssignBatchService();
                    await _delayManager.WaitAsync(stoppingToken);
                    continue;
                }

                // === STAGE 1: ATOMICALLY CLAIM A WORK GROUP ===
                // 1. Get a list of all potential work groups
                var potentialWorkKeys = await _batchService.GetPotentialWorkGroups(stoppingToken);
                var lockedGroups = new List<string>();
                // 2. Iterate through the potential groups and ATTEMPT to lock the first one we can.
                foreach (var key in potentialWorkKeys.Shuffle()) // Randomize the order to avoid thundering herd.
                {
                    if (!await _batchService.AcquireLock(key, 10)) continue;
                    // SUCCESS! We got the lock.
                    lockedGroups.Add(key);
                    break; // We successfully got a lock, exit the loop.
                }

                // === STAGE 2: FETCH AND PROCESS BATCH FOR THE CLAIMED GROUPS ===
                if (lockedGroups.Count > 0)
                {
                    // This method now contains all the logic for fetching, processing, and releasing the lock.
                    await ProcessGroupedBatch(lockedGroups, stoppingToken);
                }
                else
                {
                    // No grouped work was found or could be locked.
                    await _delayManager.WaitAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) // Normal shutdown
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred in the processing of SuiBackgroundService");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    public void ResetDelay()
    {
        _delayManager.Reset();
    }

    // This is core processing logic that runs AFTER a lock is secured
    private async Task ProcessGroupedBatch(List<string> concurrencyKeys, CancellationToken cancellationToken)
    {
        if (_batchService is null)
        {
            await TryReleaseLock(concurrencyKeys);
            return;
        }

        try
        {
            // Fetch a batch of transactions matching the locked key
            var transactions = await _batchService.FetchBatch(concurrencyKeys, cancellationToken);
            if (transactions.Count > 0)
            {
                _logger.LogInformation("Processing batch for keys {Key}", string.Join(',',concurrencyKeys));
                await _batchService.StartBatch(transactions);
            }
        }
        catch (Exception)
        {
            await TryReleaseLock(concurrencyKeys);
        }
    }

    private Task TryReleaseLock(string? lockedKey)
    {
        if (lockedKey is null || _batchService is null) return Task.CompletedTask;
        return _batchService.ReleaseLock(lockedKey);
    }

    private Task TryReleaseLock(List<string>? lockedKeys)
    {
        if (lockedKeys is null || lockedKeys.Count == 0 || _batchService is null) return Task.CompletedTask;
        return _batchService.ReleaseLock(lockedKeys);
    }

    private void AssignBatchService()
    {
        if (_batchService is not null) return;
        if (!BackgroundServiceState.Initialized) return;
        var service = BackgroundServiceState.GetFromMicroservice<TransactionBatchService>();
        if (service is null) return;
        _batchService = service;
    }
}