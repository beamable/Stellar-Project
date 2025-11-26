using System.Threading.Tasks;
using Beamable.Server;
using Beamable.StellarFederation.Features.Scheduler.Storage.Modles;
using MongoDB.Driver;

namespace Beamable.StellarFederation.Features.Scheduler.Storage;

public class BlockCollection(IStorageObjectConnectionProvider storageObjectConnectionProvider)
    : IService
{
    private IMongoCollection<Block>? _collection;

    private async ValueTask<IMongoCollection<Block>> Get()
    {
        if (_collection is null)
        {
            var db = await storageObjectConnectionProvider.StellarFederationStorageDatabase();
            _collection = db.GetCollection<Block>("block");
        }

        return _collection;
    }

    public async Task<long> InsertBlock(string network, long blockNumber)
    {
        var collection = await Get();
        var update = Builders<Block>.Update.Set(x => x.BlockNumber, blockNumber);

        var options = new FindOneAndUpdateOptions<Block>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        var updated = await collection.FindOneAndUpdateAsync(x => x.Network == network, update, options);
        return updated.BlockNumber;
    }

    public async Task<Block?> Get(string network)
    {
        var collection = await Get();
        var block = await collection
            .Find(x => x.Network == network)
            .SingleOrDefaultAsync();
        return block;
    }
}