using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;
using Beamable.StellarFederation.Features.Contract.Storage.Models;
using Beamable.StellarFederation.Features.Minting;
using Beamable.StellarFederation.Features.Minting.Storage;
using Beamable.StellarFederation.Features.Minting.Storage.Models;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class NftCreateHandler(
    CounterCollection counterCollection,
    MetadataService metadataService,
    ContractService contractService,
    ContractProxy contractProxy,
    MintCollection mintCollection) : IService, ITransactionHandler
{
    public async Task HandleAsync(List<QueuedTransactionBase> transactions)
    {
        var typedTransactions = transactions.Cast<ItemAddInventoryRequest>().ToList();
        var mints = new List<Mint>();
        foreach (var transaction in typedTransactions)
        {
            var contract = await contractService.GetByContentId<ItemContract>(transaction.ContentId.ToContentType());
            var tokenId = await counterCollection.GetNextCounterValue(transaction.ContentId.ToContentType());
            var metadata = await metadataService.BuildMetadata(tokenId, transaction.ContentId, transaction.Properties);
            var metadataHash = await metadataService.SaveMetadata(metadata);
            mints.Add(new Mint
            {
                ContentId = transaction.ContentId,
                ContractName = contract.Address,
                TokenId = tokenId,
                Metadata = metadata,
                InitialOwnerAddress = transaction.Wallet,
                MetadataHash = metadataHash,
                Amount = 1
            });
        }

        var functionMessage = new MintItemFunctionMessage(
            typedTransactions.Select(x => x.TransactionId).ToArray(),
            typedTransactions.First().ContentId.ToContentType(),
            typedTransactions.First().ContentId.ToContentType(),
            typedTransactions.Select(x => x.Wallet).ToArray(),
            mints.Select(x => x.TokenId).ToArray(),
            mints.Select(x => x.MetadataHash).ToArray()
            );
        var transactionHash = await contractProxy.ItemBatchMint(functionMessage);
        mints.ForEach(x => x.TransactionHash = transactionHash);
        await mintCollection.InsertMints(mints);
    }

}