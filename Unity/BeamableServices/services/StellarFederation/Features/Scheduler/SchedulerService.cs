using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api;
using Beamable.Common.Scheduler;
using Beamable.Server;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.Scheduler.Storage;
using Beamable.StellarFederation.Features.Scheduler.Storage.Modles;
using Beamable.StellarFederation.Features.Stellar;
using beamable.tooling.common.Scheduler;
using StellarFederationCommon;
using StellarFederationCommon.Models.Response;

namespace Beamable.StellarFederation.Features.Scheduler;

public class SchedulerService(
    BeamScheduler beamScheduler,
    BlockCollection blockCollection,
    Configuration configuration,
    StellarService stellarService,
    IBeamableRequester beamableRequester)
    : IService
{
    private const string ChainProcessorName = "chainBlockProcessor";

    public async Task<SchedulerJobResponse> Start()
    {
        try
        {
            var latestBlockHorizon = await blockCollection.Get(await configuration.StellarRpc, StellarSettings.HorizonApi);
            var latestBlockSoroban = await blockCollection.Get(await configuration.StellarRpc, StellarSettings.SorobanApi);
            var block = await stellarService.GetCurrentLedgerSequence();
            if (latestBlockHorizon is null)
            {
                await blockCollection.InsertHorizonBlock(block);
            }
            if (latestBlockSoroban is null)
            {
                await blockCollection.InsertSorobanBlock(block);
            }

            return await StartJob();
        }
        catch (Exception e)
        {
            BeamableLogger.LogError("Can't start scheduler", e.ToLogFormat());
            return SchedulerJobResponse.Empty();
        }
    }

    public async Task<SchedulerJobResponse> End()
    {
        try
        {
            var names = new List<string>();
            var existingJobs = beamScheduler.GetAllJobs(GlobalChainSource());
            await foreach (var job in existingJobs)
            {
                names.Add(job.name);
                await beamScheduler.DeleteJob(job.id);
            }
            return new SchedulerJobResponse
            {
                names = names.ToArray()
            };
        }
        catch (Exception e)
        {
            BeamableLogger.LogError("Can't end scheduler", e.ToLogFormat());
            return SchedulerJobResponse.Empty();
        }
    }

    private async Task<SchedulerJobResponse> StartJob()
    {
        var existingJobs = await beamScheduler.GetAllJobs(GlobalChainSource()).ToListAsync();

        if (existingJobs.IsNullOrEmpty())
        {
            var cronSeconds = await configuration.FetchLogsCronSeconds;
            var job = await beamScheduler.Schedule()
                .Microservice<StellarFederation>()
                .Run(s => s.BlockProcessor)
                .WithRetryPolicy(new RetryPolicy { maxRetryCount = 0, retryDelayMs = 0, useExponentialBackoff = false })
                .OnCron(c => c.EveryNthSecond(cronSeconds).EveryMinute().EveryHour().EveryDay().EveryMonth())
                .Save(ChainJobName(), GlobalChainSource(), true);
            return new SchedulerJobResponse
            {
                names = [job.name]
            };
        }
        return SchedulerJobResponse.Empty();
    }

    private string GlobalChainSource() => $"{StellarFederationSettings.MicroserviceName}.{beamableRequester.Cid}.{beamableRequester.Pid}";
    private string ChainJobName() => $"{GlobalChainSource()}.{ChainProcessorName}";

}