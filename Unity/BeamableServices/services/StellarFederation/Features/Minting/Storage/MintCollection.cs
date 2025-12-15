using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Server;
using Beamable.StellarFederation.Features.Minting.Models;
using Beamable.StellarFederation.Features.Minting.Storage.Models;
using MongoDB.Driver;

namespace Beamable.StellarFederation.Features.Minting.Storage;

public class MintCollection : IService
{
    private readonly IStorageObjectConnectionProvider _storageObjectConnectionProvider;
	private IMongoCollection<Mint>? _collection;

	public MintCollection(IStorageObjectConnectionProvider storageObjectConnectionProvider)
	{
		_storageObjectConnectionProvider = storageObjectConnectionProvider;
	}

	private async ValueTask<IMongoCollection<Mint>> Get()
	{
		if (_collection is null)
		{
			var db = await _storageObjectConnectionProvider.StellarFederationStorageDatabase();
			_collection = db.GetCollection<Mint>("mint");
			await _collection.Indexes.CreateManyAsync([
				new CreateIndexModel<Mint>(Builders<Mint>.IndexKeys.Ascending(x => x.ContractName).Ascending(x => x.ContentId).Ascending(x => x.TokenId).Ascending(x => x.TransactionHash), new CreateIndexOptions { Unique = true }),
				new CreateIndexModel<Mint>(Builders<Mint>.IndexKeys.Ascending(x => x.ContractName).Ascending(x => x.TokenId).Ascending(x => x.ContentId)),
				new CreateIndexModel<Mint>(Builders<Mint>.IndexKeys.Ascending(x => x.ContractName).Ascending(x => x.InitialOwnerAddress).Ascending(x => x.ContentId)),
				new CreateIndexModel<Mint>(Builders<Mint>.IndexKeys.Ascending(x => x.TransactionHash)),
				new CreateIndexModel<Mint>(Builders<Mint>.IndexKeys.Ascending(x => x.ContractName).Ascending(x => x.TokenId).Ascending(x => x.MintState))
			]);
		}

		return _collection;
	}

	public async Task<List<TokenIdMapping>> GetTokenMappingsForContent(string contractName, IEnumerable<string> contentIds)
	{
		var collection = await Get();
		var mints = await collection.Find(x => x.ContractName == contractName && contentIds.Contains(x.ContentId)).Project(x => new TokenIdMapping
		{
			ContentId = x.ContentId,
			TokenId = x.TokenId,
			MetadataHash = x.MetadataHash
		}).ToListAsync();

		return mints
			.GroupBy(x => new { x.ContentId, x.TokenId, x.MetadataHash })
			.Select(group => group.First())
			.ToList();
	}

	public async Task<List<Mint>> GetTokenMints(string contractName, IEnumerable<uint> tokenIds, MintState mintState = MintState.Created)
	{
		var collection = await Get();
		var mints = await collection
			.Find(x => x.ContractName == contractName && tokenIds.Contains(x.TokenId) && x.MintState == mintState)
			.ToListAsync();
		return mints;
	}

	public async Task<Mint> GetTokenMint(string contractName, string contentId, MintState mintState = MintState.Created)
	{
		var collection = await Get();
		var mint = await collection
			.Find(x => x.ContractName == contractName && x.ContentId == contentId && x.MintState == mintState)
			.FirstOrDefaultAsync();
		return mint;
	}

	public async Task<Mint?> GetTokenMint(string contractName, uint tokenId, MintState mintState = MintState.Created)
	{
		var collection = await Get();
		return await collection.Find(x => x.ContractName == contractName && tokenId == x.TokenId && x.MintState == mintState)
			.FirstOrDefaultAsync();
	}

	public async Task InsertMints(IEnumerable<Mint> mints)
	{
		var collection = await Get();
		var options = new InsertManyOptions
		{
			IsOrdered = false
		};
		await collection.InsertManyAsync(mints, options);
	}

	public async Task SaveMetadata(UpdateMetadataRequest request)
	{
		var collection = await Get();
		await collection.UpdateManyAsync(
			x => x.TokenId == request.TokenId,
			Builders<Mint>.Update.Set(x => x.Metadata, request.Metadata));
	}

	public async Task BulkSaveMetadata(IList<UpdateMetadataRequest> requests)
	{
		var collection = await Get();

		var updates = requests.Select(request =>
			new UpdateManyModel<Mint>(
				Builders<Mint>.Filter.Eq(x => x.TokenId, request.TokenId),
			    Builders<Mint>.Update
				    .Set(x => x.Metadata, request.Metadata)
				    .Set(x => x.MetadataHash, request.MetadataHash)
				    .Set(x => x.MintState, MintState.Modified)
				)).ToList();

		await collection.BulkWriteAsync(updates, new BulkWriteOptions
		{
			IsOrdered = false
		});
	}

	public async Task UpdateMintState(uint[] ids, MintState mintState)
	{
		if (ids.Length == 0)
			return;

		var collection = await Get();
		var filter = Builders<Mint>.Filter.In(x => x.TokenId, ids);
		var update = Builders<Mint>.Update.Set(x => x.MintState, mintState);
		await collection.UpdateManyAsync(filter, update);
	}
}