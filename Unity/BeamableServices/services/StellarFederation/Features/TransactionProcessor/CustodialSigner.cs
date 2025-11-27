using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Accounts.Models;
using StellarDotnetSdk.Transactions;

namespace Beamable.StellarFederation.Features.TransactionProcessor;

public class CustodialSigner : ITransactionSigner
{
    public Task Sign(Transaction transaction, List<Account> accounts)
    {
        foreach (var account in accounts)
        {
            transaction.Sign(account.KeyPair);
        }
        return Task.CompletedTask;
    }
}