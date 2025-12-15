using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Functions.TransferFrom.Models;
using Beamable.StellarFederation.Features.Stellar;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class CoinCurrencySubtractHandler(
    ContractProxy contractProxy,
    AccountsService accountsService,
    StellarService stellarService
    ) : IService, ITransactionHandler
{
    public async Task HandleAsync(List<QueuedTransactionBase> transactions)
    {
        var typedTransactions = transactions.Cast<CurrencySubtractInventoryRequest>().ToList();
        var contractAccount = await accountsService.GetAccount(typedTransactions.First().ContentId);
        var latestLedger = await stellarService.GetCurrentLedgerSequence();
        var liveUntil = StellarServiceExtensions.ExpiresInDays(latestLedger);
        var functionMessage = new CoinTransferFromUserFunctionMessage
        (typedTransactions.Select(x => x.TransactionId).ToArray(),
            typedTransactions.First().ContentId,
            typedTransactions.First().ContentId.ToCurrencyModuleName(),
            typedTransactions.Select(x => x.GamerTag).ToArray(),
            typedTransactions.Select(x => x.Wallet).ToArray(),
            Enumerable.Repeat(contractAccount!.Value.Address, typedTransactions.Count).ToArray(),
            typedTransactions.Select(x => x.Amount).ToArray(),
            Enumerable.Repeat(liveUntil, typedTransactions.Count).ToArray());

        await contractProxy.CoinTransferFromUser(functionMessage);
    }
}