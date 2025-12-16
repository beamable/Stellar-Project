using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Accounts.Models;
using StellarDotnetSdk.Transactions;

namespace Beamable.StellarFederation.Features.TransactionProcessor;

public interface ITransactionSigner
{
    Task Sign(Transaction transaction, List<Account> accounts);
}