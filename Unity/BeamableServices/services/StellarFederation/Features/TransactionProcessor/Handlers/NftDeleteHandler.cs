using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;
using Beamable.StellarFederation.Features.Minting.Storage;
using Beamable.StellarFederation.Features.Minting.Storage.Models;
using Beamable.StellarFederation.Features.Stellar;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class NftDeleteHandler(
    ContractProxy contractProxy,
    MintCollection mintCollection,
    StellarService stellarService) : IService, ITransactionHandler
{
    public async Task HandleAsync(List<QueuedTransactionBase> transactions)
    {
        var typedTransactions = transactions.Cast<ItemDeleteInventoryRequest>().ToList();
        var latestLedger = await stellarService.GetCurrentLedgerSequence();
        var liveUntil = StellarServiceExtensions.ExpiresInDays(latestLedger);
        var functionMessage = new DeleteItemFunctionMessage(
            typedTransactions.Select(x => x.TransactionId).ToArray(),
            typedTransactions.First().ContentId.ToContentType(),
            typedTransactions.First().ContentId.ToCurrencyModuleName(),
            typedTransactions.Select(x => x.GamerTag).ToArray(),
            typedTransactions.Select(x => x.Wallet).ToArray(),
            typedTransactions.Select(x => uint.Parse(x.ProxyId)).ToArray(),
            Enumerable.Repeat(liveUntil, typedTransactions.Count).ToArray()
            );
        await contractProxy.ItemBatchBurn(functionMessage);
        await mintCollection.UpdateMintState(typedTransactions.Select(x => uint.Parse(x.ProxyId)).ToArray(), MintState.Deleted);
    }

}