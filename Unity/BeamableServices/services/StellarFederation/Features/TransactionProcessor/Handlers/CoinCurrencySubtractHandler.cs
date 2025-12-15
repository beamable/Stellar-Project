using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Functions.TransferFrom.Models;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class CoinCurrencySubtractHandler(
    ContractProxy contractProxy,
    AccountsService accountsService
    ) : IService, ITransactionHandler
{
    public async Task HandleAsync(List<QueuedTransactionBase> transactions)
    {
        var typedTransactions = transactions.Cast<CurrencySubtractInventoryRequest>().ToList();
        var transferFromMessages = new List<CoinTransferFromFunctionMessageSingle>();
        foreach (var transaction in typedTransactions)
        {
            var allowance = await contractProxy.GetCoinAllowance(transaction.Wallet, transaction.ContentId);
            if (allowance.Amount >= transaction.Amount)
            {
                var contractAccount = await accountsService.GetAccount(transaction.ContentId);
                transferFromMessages.Add(new CoinTransferFromFunctionMessageSingle(
                    transaction.TransactionId,
                    transaction.ContentId,
                    transaction.ConcurrencyKey,
                    contractAccount!.Value.Address,
                    transaction.Wallet,
                    contractAccount!.Value.Address,
                    transaction.Amount
                    ));
            }
        }

        if (transferFromMessages.Count > 0)
        {
            var message = new CoinTransferFromFunctionMessage(
                TransactionIds: transferFromMessages.Select(x => x.TransactionIds).ToArray(),
                ContentId: transferFromMessages.First().ContentId,
                ConcurrencyKey: transferFromMessages.First().ConcurrencyKey,
                Spender: transferFromMessages.Select(x => x.Spender).ToArray(),
                From: transferFromMessages.Select(x => x.From).ToArray(),
                To: transferFromMessages.Select(x => x.To).ToArray(),
                Amount: transferFromMessages.Select(x => x.Amount).ToArray()
            );
            await contractProxy.TransferFrom(message);
        }
    }
}