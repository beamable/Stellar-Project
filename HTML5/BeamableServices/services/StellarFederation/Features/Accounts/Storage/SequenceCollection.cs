using System.Linq;
using System.Threading.Tasks;
using Beamable.Server;
using Beamable.StellarFederation.Features.Accounts.Storage.Models;
using MongoDB.Driver;

namespace Beamable.StellarFederation.Features.Accounts.Storage;

public class SequenceCollection : IService
{
    private readonly IStorageObjectConnectionProvider _storageObjectConnectionProvider;
	private IMongoCollection<Sequence>? _collection;

	public SequenceCollection(IStorageObjectConnectionProvider storageObjectConnectionProvider)
	{
		_storageObjectConnectionProvider = storageObjectConnectionProvider;
	}

	private async ValueTask<IMongoCollection<Sequence>> Get()
	{
		if (_collection is null)
		{
			var db = await _storageObjectConnectionProvider.StellarFederationStorageDatabase();
			_collection = db.GetCollection<Sequence>("sequence");
		}

		return _collection;
	}

	public async Task<long?> GetNextIfNoErrors(string address)
	{
		var collection = await Get();
		var update = Builders<Sequence>.Update.Inc(x => x.State, 1);

		var options = new FindOneAndUpdateOptions<Sequence>
		{
			ReturnDocument = ReturnDocument.After,
			IsUpsert = false
		};

		var updated = await collection.FindOneAndUpdateAsync(x => x.Address == address && x.Errors.Count == 0,
			update,
			options);

		return updated?.State;
	}

	public async Task<long?> PopError(string address)
	{
		var collection = await Get();

		var update = Builders<Sequence>.Update.PopFirst(x => x.Errors);
		var options = new FindOneAndUpdateOptions<Sequence>
		{
			ReturnDocument = ReturnDocument.Before,
			IsUpsert = false
		};
		var updated = await collection.FindOneAndUpdateAsync(x => x.Address == address && x.Errors.Count > 0,
			update,
			options);

		return updated?.Errors.First();
	}

	public async Task PushError(string address, long value)
	{
		var collection = await Get();
		var update = Builders<Sequence>.Update
			.Push(x => x.Errors, value);

		var options = new UpdateOptions
		{
			IsUpsert = false
		};

		await collection.UpdateOneAsync(x => x.Address == address, update, options);
	}

	public async Task Set(string address, long value)
	{
		var collection = await Get();
		var update = Builders<Sequence>.Update
			.Set(x => x.State, value)
			.Set(x => x.Errors, []);

		var options = new UpdateOptions
		{
			IsUpsert = true
		};

		await collection.UpdateOneAsync(x => x.Address == address, update, options);
	}
}