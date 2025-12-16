using System.Threading.Tasks;
using Beamable.Server;
using Beamable.StellarFederation.Features.Minting.Storage.Models;
using MongoDB.Driver;

namespace Beamable.StellarFederation.Features.Minting.Storage;

public class CounterCollection(IStorageObjectConnectionProvider storageObjectConnectionProvider)
    : IService
{
    private IMongoCollection<Counter>? _collection;

    private async ValueTask<IMongoCollection<Counter>> Get()
    {
        if (_collection is null)
        {
            var db = await storageObjectConnectionProvider.StellarFederationStorageDatabase();
            _collection = db.GetCollection<Counter>("counter");
        }

        return _collection;
    }

    public async Task<uint> GetNextCounterValue(string counterName)
    {
        var collection = await Get();
        var update = Builders<Counter>.Update.Inc(x => x.State, (uint)1);

        var options = new FindOneAndUpdateOptions<Counter>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        var updated = await collection.FindOneAndUpdateAsync<Counter>(x => x.Name == counterName, update, options);

        return updated.State;
    }
}