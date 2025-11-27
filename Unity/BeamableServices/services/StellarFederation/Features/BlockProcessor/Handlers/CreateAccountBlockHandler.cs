using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Caching;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Accounts.Storage;
using Beamable.StellarFederation.Features.Contract.Functions.Account.Models;
using Beamable.StellarFederation.Features.Notifications;
using Beamable.StellarFederation.Features.Stellar;
using Beamable.StellarFederation.Features.Transactions.Storage;
using SuiFederationCommon.Models.Notifications;

namespace Beamable.StellarFederation.Features.BlockProcessor.Handlers;

public class CreateAccountBlockHandler(
    StellarService stellarService,
    AccountsService accountsService,
    TransactionLogCollection transactionLogCollection,
    VaultCollection vaultCollection,
    TransactionQueueCollection transactionQueueCollection,
    PlayerNotificationService playerNotificationService) : IService
{
    public async Task Handle(uint fromBlock, uint toBlock)
    {
        var realmAccount = await accountsService.GetOrCreateRealmAccount();
        var logs = await stellarService.GetHorizonLogs(realmAccount.Address, fromBlock, toBlock, CreateAccountFunctionMessage.StaticFunctionName);
        var transactionLogs = await transactionLogCollection.GetByChainTransactionHashes(logs.Select(l => l.Hash));
        await transactionLogCollection.SetMintedDone(transactionLogs.Select(x => x.Id));
        await vaultCollection.SetCreated(transactionLogs.Select(x => x.PlayerUserId.ToString()));
        await transactionQueueCollection.Delete(transactionLogs.Select(x => x.Id));
        GlobalCache.RemoveAccountCache(transactionLogs.Select(x => x.PlayerUserId.ToString()));
        _ = playerNotificationService.SendPlayerNotification(transactionLogs.Select(x => x.PlayerUserId),
            new CustodialAccountCreatedNotification());
    }
}