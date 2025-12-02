using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Accounts.Models;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Functions.Account.Models;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class AccountCreateHandler(ContractProxy contractProxy, AccountsService accountsService) : IService, ITransactionHandler
{
    private readonly CustodialSigner _signer = new();

    public async Task HandleAsync(List<QueuedTransactionBase> transactions)
    {
        var typedTransactions = transactions.Cast<AccountCreateRequest>().ToList();
        var tasks = typedTransactions.Select(async x =>
            new CreateAccountFunctionMessage(
                x.TransactionId,
                (Account)(await accountsService.GetAccount(x.GamerTag.ToString()))!
            )
        );
        var functionMessages = await Task.WhenAll(tasks);
        var transaction = await contractProxy.PrepareCreateAccountWithSponsorship(functionMessages);
        await _signer.Sign(transaction, functionMessages.Select(x => x.NewAccount).ToList());
        await contractProxy.SendCreateAccountWithSponsorship(transaction, functionMessages);
    }
}