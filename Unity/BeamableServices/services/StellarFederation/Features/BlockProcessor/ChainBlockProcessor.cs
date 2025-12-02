using System;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.Scheduler.Storage;
using Beamable.StellarFederation.Features.Stellar;

namespace Beamable.StellarFederation.Features.BlockProcessor;

public class ChainBlockProcessor(
    BlockCollection blockCollection,
    Configuration configuration,
    StellarService stellarService,
    NativeBlockProcessor nativeBlockProcessor) : IService
{

    public async Task Start()
    {
        var fromBlockModel = await blockCollection.Get(await configuration.StellarRpc);
        var fromBlock = fromBlockModel!.BlockNumber;
        var toBlock = await stellarService.GetCurrentLedgerSequence();
        var maxLogsBlockSize = await configuration.FetchLogsBlockSize;
        if (toBlock - fromBlock > maxLogsBlockSize - 1)
        {
            toBlock = fromBlock + maxLogsBlockSize - 1;
        }
        if (fromBlock <= toBlock)
        {
            await ProcessLogs(fromBlock, toBlock);
            await blockCollection.InsertBlock(await configuration.StellarRpc, toBlock);
        }
    }

    private async Task ProcessLogs(uint fromBlock, uint toBlock)
    {
        try
        {
            await nativeBlockProcessor.Process(fromBlock, toBlock);
        }
        catch (Exception ex)
        {
            BeamableLogger.LogError($"ChainBlockProcessor: Error in ProcessLogs: {ex.Message}");
        }
    }
}