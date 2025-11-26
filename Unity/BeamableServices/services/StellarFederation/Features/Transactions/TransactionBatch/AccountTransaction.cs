using System;
using System.Threading.Tasks;
using Beamable.StellarFederation.BackgroundService;
using Beamable.StellarFederation.Extensions;
using Beamable.StellarFederation.Features.Accounts.Models;
using Beamable.StellarFederation.Features.Transactions.Models;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;

namespace Beamable.StellarFederation.Features.Transactions;

public partial class TransactionBatchService
{
    public async Task Insert(Account account, MicroserviceInfo microserviceInfo)
    {
        var uniqueTransactionId = Guid.NewGuid().ToString();
        var transactionId = await _transactionManager.StartTransaction(new NewCustomTransaction(account.Name.ToLong(), account.Address, nameof(AccountCreateRequest)));
        _ = _transactionManager.RunAsyncBlock(transactionId, uniqueTransactionId, async () =>
        {
            long.TryParse(account.Name, out var gamerTag);
            await _transactionQueueCollection.Insert(new AccountCreateRequest
            {
                Wallet = account.Address,
                GamerTag = gamerTag,
                UniqueTransactionId = uniqueTransactionId,
                TransactionId = transactionId,
                ConcurrencyKey = nameof(AccountCreateRequest),
                FunctionName = nameof(AccountCreateRequest),
                MicroserviceInfo = microserviceInfo,
                Status = TransactionStatus.Pending
            });
            BackgroundServiceState.ResetDelay();
        });
    }
}