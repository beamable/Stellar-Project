using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.LockManager;
using Beamable.StellarFederation.Features.Transactions.Storage;
using StellarDotnetSdk.Responses.SorobanRpc;

namespace Beamable.StellarFederation.Features.BlockProcessor.Handlers;

public class TransferBlockHandler(
    TransactionLogCollection transactionLogCollection,
    UpdatePlayerStateService playerStateService,
    TransactionQueueCollection transactionQueueCollection,
    LockManagerService lockManagerService) : IService
{
    public async Task Handle(List<GetEventsResponse.EventInfo> logsEvents)
    {
        var transactionLogs = await transactionLogCollection.GetByChainTransactionHashes(logsEvents.Select(l => l.TransactionHash));
        await transactionLogCollection.SetMintedDone(transactionLogs.Select(x => x.Id));
        await transactionQueueCollection.Delete(transactionLogs.Select(x => x.Id));
        await lockManagerService.ReleaseLock(transactionLogs.SelectMany(x => x.ConcurrencyKey));
        await playerStateService.Update(transactionLogs.Select(x => x.PlayerUserId));
    }
}