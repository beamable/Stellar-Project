using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class CoinCurrencyAddHandler(ContractProxy contractProxy) : IService, ITransactionHandler
{
    public async Task HandleAsync(List<QueuedTransactionBase> transactions)
    {
        var typedTransactions = transactions.Cast<CurrencyAddInventoryRequest>().ToList();
        var functionMessage = new MintCoinFunctionMessage
            (typedTransactions.Select(x => x.TransactionId).ToArray(),
                typedTransactions.First().ContentId,
                typedTransactions.First().ContentId.ToCurrencyModuleName(),
                typedTransactions.Select(x => x.Wallet).ToArray(),
                typedTransactions.Select(x => x.Amount).ToArray());

        await contractProxy.CoinBatchMint(functionMessage);
    }
}