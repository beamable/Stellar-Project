using System;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.BlockProcessor.Decoder;
using Beamable.StellarFederation.Features.BlockProcessor.Handlers;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Scheduler.Storage;
using Beamable.StellarFederation.Features.Stellar;

namespace Beamable.StellarFederation.Features.BlockProcessor;

public class SorobanBlockProcessor(
    StellarService stellarService,
    BlockCollection blockCollection,
    Configuration configuration,
    ContractService contractService,
    TransferBlockHandler transferBlockHandler) : IService
{
    public async Task Handle()
    {
        var fromBlockModel = await blockCollection.Get(await configuration.StellarRpc, StellarSettings.SorobanApi);
        if (fromBlockModel is null)
            return;
        try
        {
            var allContracts = await contractService.GetAllContracts();
            var logs = await stellarService.GetSorobanLogs(fromBlockModel,
                allContracts.Select(x => x.Address).ToArray());
            if (logs.Events.Count == 0)
            {
                var block = await stellarService.GetCurrentLedgerSequence();
                fromBlockModel.BlockNumber = block;
                return;
            }
            fromBlockModel.BlockNumber = logs.LastProcessedLedger;
            fromBlockModel.Cursor = logs.LastCursor;

            //var mintDecoder = new SorobanEventDecoder<MintEventDto>("mint");
            //var mintEvents = mintDecoder.DecodeEvents(logs.Events);

            await transferBlockHandler.Handle(logs.Events);

        }
        catch (Exception e)
        {
            BeamableLogger.LogWarning("Exception in HorizonBlockProcessor", e.ToLogFormat());
        }
        finally
        {
            await blockCollection.InsertBlock(fromBlockModel);
        }
    }
}