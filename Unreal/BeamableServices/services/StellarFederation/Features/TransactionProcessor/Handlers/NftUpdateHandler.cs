using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.Contract;
using Beamable.StellarFederation.Features.Contract.Functions.Minting.Models;
using Beamable.StellarFederation.Features.Minting;
using Beamable.StellarFederation.Features.Minting.Models;
using Beamable.StellarFederation.Features.Minting.Storage;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Features.TransactionProcessor.Handlers;

public class NftUpdateHandler(
    MetadataService metadataService,
    ContractProxy contractProxy,
    MintCollection mintCollection) : IService, ITransactionHandler
{
    public async Task HandleAsync(List<QueuedTransactionBase> transactions)
    {
        var typedTransactions = transactions.Cast<ItemUpdateInventoryRequest>().ToList();
        var mintUpdates = new List<UpdateMetadataRequest>();
        foreach (var transaction in typedTransactions)
        {
            var metadata = await metadataService.BuildMetadata(uint.Parse(transaction.ProxyId), transaction.ContentId, transaction.Properties);
            var metadataHash = await metadataService.SaveMetadata(metadata);
            mintUpdates.Add(new UpdateMetadataRequest
            {
                TokenId = uint.Parse(transaction.ProxyId),
                Metadata = metadata,
                MetadataHash = metadataHash
            });
        }

        var functionMessage = new UpdateItemFunctionMessage(
            typedTransactions.Select(x => x.TransactionId).ToArray(),
            typedTransactions.First().ContentId.ToContentType(),
            typedTransactions.First().ContentId.ToCurrencyModuleName(),
            mintUpdates.Select(x => x.TokenId).ToArray(),
            mintUpdates.Select(x => x.MetadataHash).ToArray()
            );
        await contractProxy.ItemBatchUpdate(functionMessage);
        await mintCollection.BulkSaveMetadata(mintUpdates);
    }

}