using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Server;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Beamable.StellarFederation.Features.Transactions.Storage;

public class TransactionLogCollection : IService
{
	private readonly IStorageObjectConnectionProvider _storageObjectConnectionProvider;
	private IMongoCollection<TransactionLog>? _collection;

	public TransactionLogCollection(IStorageObjectConnectionProvider storageObjectConnectionProvider)
	{
		_storageObjectConnectionProvider = storageObjectConnectionProvider;
	}

	private async ValueTask<IMongoCollection<TransactionLog>> Get()
	{
		if (_collection is null)
		{
			var db = await _storageObjectConnectionProvider.StellarFederationStorageDatabase();
			_collection = db.GetCollection<TransactionLog>("transaction-log");

			await _collection.Indexes.CreateOneAsync(new CreateIndexModel<TransactionLog>(Builders<TransactionLog>.IndexKeys
				.Ascending(x => x.InventoryTransactionId)
			));

			await _collection.Indexes.CreateOneAsync(new CreateIndexModel<TransactionLog>(Builders<TransactionLog>.IndexKeys
				.Ascending(x => x.Wallet)
				.Ascending(x => x.OperationName)
			));
			await _collection.Indexes.CreateOneAsync(new CreateIndexModel<TransactionLog>(Builders<TransactionLog>.IndexKeys
				.Ascending("ChainTransactions.Hash")
			));
			await _collection.Indexes.CreateOneAsync(new CreateIndexModel<TransactionLog>(Builders<TransactionLog>.IndexKeys
				.Ascending(x => x.MintedTimestamp)
				.Ascending("ChainTransactions.Hash"), new CreateIndexOptions
				{
					Sparse = true
				}
			));
		}
		return _collection;
	}

	public async Task Insert(TransactionLog log)
	{
		var collection = await Get();
		await collection.InsertOneAsync(log);
	}

	public async Task SetDone(ObjectId inventoryTransaction)
	{
		var collection = await Get();
		var update = Builders<TransactionLog>.Update.Set(x => x.EndTimestamp, DateTime.UtcNow);
		await collection.UpdateOneAsync(x => x.Id == inventoryTransaction, update);
	}

	public async Task SetMintedDone(ObjectId objectId)
	{
		var collection = await Get();
		var update = Builders<TransactionLog>.Update.Set(x => x.MintedTimestamp, DateTime.UtcNow);
		await collection.UpdateOneAsync(x => x.Id == objectId, update);
	}

	public async Task SetMintedDone(IEnumerable<ObjectId> objectIds)
	{
		var collection = await Get();
		var filter = Builders<TransactionLog>.Filter.In(x => x.Id, objectIds);
		var update = Builders<TransactionLog>.Update
			.Set(x => x.MintedTimestamp, DateTime.UtcNow);
		await collection.UpdateManyAsync(filter, update);
	}

	public async Task SetError(ObjectId transactionId, string error)
	{
		var collection = await Get();
		var update = Builders<TransactionLog>.Update.Set(x => x.Error, error);
		await collection.UpdateOneAsync(x => x.Id == transactionId, update);
	}

	public async Task AddChainTransaction(ObjectId transactionId, ChainTransaction chainTransaction, string? concurrencyKey = null)
	{
		var collection = await Get();
		var updates = new List<UpdateDefinition<TransactionLog>>
		{
			Builders<TransactionLog>.Update.Push(x => x.ChainTransactions, chainTransaction)
		};

		if (!string.IsNullOrWhiteSpace(concurrencyKey))
		{
			updates.Add(
				Builders<TransactionLog>.Update.AddToSet(x => x.ConcurrencyKey, concurrencyKey)
			);
		}

		var update = Builders<TransactionLog>.Update.Combine(updates);

		await collection.UpdateOneAsync(x => x.Id == transactionId, update);
	}

	public async Task<string> GetInventoryTransaction(ObjectId id)
	{
		var collection = await Get();
		return (await collection.Find(x => x.Id == id).FirstOrDefaultAsync()).InventoryTransactionId ?? "";
	}

	public async Task<TransactionLog?> GetByInventoryTransaction(string inventoryTransaction)
	{
		var collection = await Get();
		return await collection.Find(x => x.InventoryTransactionId == inventoryTransaction).FirstOrDefaultAsync();
	}

	public async Task<TransactionLog?> GetByChainTransactionHash(string hash)
	{
		var collection = await Get();
		return await collection
			.Find(Builders<TransactionLog>.Filter.ElemMatch(x => x.ChainTransactions, xx => xx.Hash == hash))
			.FirstOrDefaultAsync();
	}

	public async Task<List<TransactionLog>> GetByChainTransactionHashes(IEnumerable<string> hashes)
	{
		var collection = await Get();
		var filter = Builders<TransactionLog>.Filter.And(
			Builders<TransactionLog>.Filter.Where(x => x.MintedTimestamp == null),
			Builders<TransactionLog>.Filter.ElemMatch(
				x => x.ChainTransactions,
				Builders<ChainTransaction>.Filter.In(tx => tx.Hash, hashes))
		);
		return await collection.Find(filter).ToListAsync();
	}

}