using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Accounts.Models;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Functions.Account.Models;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class AccountCloseHandler(ContractProxy contractProxy, AccountsService accountsService) : IService, ITransactionHandler
{
    private readonly CustodialSigner _signer = new();

    public async Task HandleAsync(List<QueuedTransactionBase> transactions)
    {
        var typedTransactions = transactions.Cast<AccountCloseRequest>().ToList();
        var tasks = typedTransactions.Select(async x =>
            new CloseAccountFunctionMessage(
                x.TransactionId,
                (Account)(await accountsService.GetAccount(x.GamerTag.ToString()))!
            )
        );
        var functionMessages = await Task.WhenAll(tasks);
        var transaction = await contractProxy.PrepareCloseAccountWithSponsorship(functionMessages);
        await _signer.Sign(transaction, functionMessages.Select(x => x.Account).ToList());
        await contractProxy.SendCloseAccountWithSponsorship(transaction, functionMessages);
    }
}