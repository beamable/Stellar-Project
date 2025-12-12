using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;
using Beamable.StellarFederation.Features.Contract.Functions.TransferFrom.Models;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class CoinCurrencySubtractHandler(ContractProxy contractProxy) : IService, ITransactionHandler
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
                //transferFromMessages.Add(new CoinTransferFromFunctionMessageSingle());
            }
        }
    }
}