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

public class SchedulerService : IService
{
    private readonly BeamScheduler _beamScheduler;
    private readonly BlockCollection _blockCollection;
    private readonly Configuration _configuration;
    private readonly StellarService _stellarService;
    private readonly IBeamableRequester _beamableRequester;

    private const string MainChainProcessor = "mainBlockProcessor";

    public SchedulerService(BeamScheduler beamScheduler, BlockCollection blockCollection, Configuration configuration, StellarService stellarService, IBeamableRequester beamableRequester)
    {
        _beamScheduler = beamScheduler;
        _blockCollection = blockCollection;
        _configuration = configuration;
        _stellarService = stellarService;
        _beamableRequester = beamableRequester;
    }

    public async Task Start()
    {
        try
        {
            var latestBlock = await _blockCollection.Get(await _configuration.StellarRpc);
            if (latestBlock is null)
            {
                var block = await _stellarService.GetCurrentLedgerSequence();
                await _blockCollection.InsertBlock(await _configuration.StellarRpc, block);
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
            var existingJobs = _beamScheduler.GetAllJobs(MainChainSource());
            await foreach (var job in existingJobs)
            {
                await _beamScheduler.DeleteJob(job.id);
            }
        }
        catch (Exception e)
        {
            BeamableLogger.LogError("Can't end scheduler", e.ToLogFormat());
        }
    }

    private async Task StartOnMainChain()
    {
        var existingJobs = await _beamScheduler.GetAllJobs(MainChainSource(), MainChainJobName()).ToListAsync();

        if (existingJobs.IsNullOrEmpty())
        {
            BeamableLogger.Log("----------------------scheduler doesnt exist");
            var cronSeconds = await _configuration.FetchLogsCronSeconds;
            await _beamScheduler.Schedule()
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

    private string MainChainSource() => $"{StellarFederationSettings.MicroserviceName}.{_beamableRequester.Cid}.{_beamableRequester.Pid}";
    private string MainChainJobName() => $"{StellarFederationSettings.MicroserviceName}.{_beamableRequester.Cid}.{_beamableRequester.Pid}.{MainChainProcessor}";

}