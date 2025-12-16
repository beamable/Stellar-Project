using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;
using Beamable.StellarFederation.Features.Contract.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class SupplyCurrencyAddHandler(
    ContractProxy contractProxy,
    AccountsService accountsService
    ) : IService, ITransactionHandler
{
    public async Task HandleAsync(List<QueuedTransactionBase> transactions)
    {
        var typedTransactions = transactions.Cast<CurrencyAddInventoryRequest>().ToList();
        var contractAccount = await accountsService.GetAccount(typedTransactions.First().ContentId);
        if (contractAccount is null)
            return;
        var functionMessage = new CoinTransferFunctionMessage
            (typedTransactions.Select(x => x.TransactionId).ToArray(),
                typedTransactions.First().ContentId,
                typedTransactions.First().ContentId.ToCurrencyModuleName(),
                Enumerable.Repeat(contractAccount.Value.Address, typedTransactions.Count).ToArray(),
                typedTransactions.Select(x => x.Wallet).ToArray(),
                typedTransactions.Select(x => x.Amount).ToArray());

        await contractProxy.CoinBatchTransfer(functionMessage);
    }
}