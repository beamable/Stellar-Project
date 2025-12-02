using System;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api;
using Beamable.Common.Scheduler;
using Beamable.Server;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Scheduler.Storage;
using Beamable.StellarFederation.Features.Stellar;
using beamable.tooling.common.Scheduler;
using StellarFederationCommon;

namespace Beamable.StellarFederation.Features.Scheduler;

public class SchedulerService(
    BeamScheduler beamScheduler,
    BlockCollection blockCollection,
    Configuration configuration,
    StellarService stellarService,
    IBeamableRequester beamableRequester)
    : IService
{
    private const string MainChainProcessor = "mainBlockProcessor";

    public async Task Start()
    {
        try
        {
            var latestBlock = await blockCollection.Get(await configuration.StellarRpc);
            if (latestBlock is null)
            {
                var block = await stellarService.GetCurrentLedgerSequence();
                await blockCollection.InsertBlock(await configuration.StellarRpc, block);
            }

            _ = StartOnMainChain();
        }
        catch (Exception e)
        {
            BeamableLogger.LogError("Can't start scheduler", e.ToLogFormat());
        }
    }

    public async Task End()
    {
        try
        {
            BeamableLogger.Log("Shutting down scheduler...");
            var existingJobs = beamScheduler.GetAllJobs(MainChainSource());
            await foreach (var job in existingJobs)
            {
                await beamScheduler.DeleteJob(job.id);
            }
        }
        catch (Exception e)
        {
            BeamableLogger.LogError("Can't end scheduler", e.ToLogFormat());
        }
    }

    private async Task StartOnMainChain()
    {
        var existingJobs = await beamScheduler.GetAllJobs(MainChainSource(), MainChainJobName()).ToListAsync();

        if (existingJobs.IsNullOrEmpty())
        {
            BeamableLogger.Log("----------------------scheduler doesnt exist");
            var cronSeconds = await configuration.FetchLogsCronSeconds;
            await beamScheduler.Schedule()
                .Microservice<StellarFederation>()
                .Run(s => s.BlockProcessor)
                .WithRetryPolicy(new RetryPolicy { maxRetryCount = 0, retryDelayMs = 0, useExponentialBackoff = false })
                .OnCron(c => c.EveryNthSecond(cronSeconds).EveryMinute().EveryHour().EveryDay().EveryMonth())
                .Save(MainChainJobName(), MainChainSource(), true);
        }
        else
        {
            BeamableLogger.Log("-------------------------scheduler exist");
        }
    }

    private string MainChainSource() => $"{StellarFederationSettings.MicroserviceName}.{beamableRequester.Cid}.{beamableRequester.Pid}";
    private string MainChainJobName() => $"{StellarFederationSettings.MicroserviceName}.{beamableRequester.Cid}.{beamableRequester.Pid}.{MainChainProcessor}";

}