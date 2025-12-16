using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Caching;
using Beamable.StellarFederation.Features.Accounts.Storage;
using Beamable.StellarFederation.Features.LockManager;
using Beamable.StellarFederation.Features.Notifications;
using Beamable.StellarFederation.Features.Transactions.Storage;
using StellarDotnetSdk.Responses;
using SuiFederationCommon.Models.Notifications;

namespace Beamable.StellarFederation.Features.BlockProcessor.Handlers;

public class CreateAccountBlockHandler(
    TransactionLogCollection transactionLogCollection,
    VaultCollection vaultCollection,
    TransactionQueueCollection transactionQueueCollection,
    PlayerNotificationService playerNotificationService,
    LockManagerService lockManagerService) : IService
{
    public async Task Handle(IEnumerable<TransactionResponse> logs)
    {
        var transactionLogs = await transactionLogCollection.GetByChainTransactionHashes(logs.Select(l => l.Hash));
        await transactionLogCollection.SetMintedDone(transactionLogs.Select(x => x.Id));
        await vaultCollection.SetCreated(transactionLogs.Select(x => x.PlayerUserId.ToString()));
        await transactionQueueCollection.Delete(transactionLogs.Select(x => x.Id));
        GlobalCache.InvalidateAccountCache(transactionLogs.Select(x => x.PlayerUserId.ToString()));
        await lockManagerService.ReleaseLock(transactionLogs.SelectMany(x => x.ConcurrencyKey));
        _ = playerNotificationService.SendPlayerNotification(transactionLogs.Select(x => x.PlayerUserId),
            new CustodialAccountCreatedNotification());
    }
}