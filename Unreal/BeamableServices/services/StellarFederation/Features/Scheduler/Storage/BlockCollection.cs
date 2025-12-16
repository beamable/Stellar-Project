using System.Threading.Tasks;
using Beamable.Server;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.Scheduler.Storage.Modles;
using MongoDB.Driver;

namespace Beamable.StellarFederation.Features.Scheduler.Storage;

public class BlockCollection(IStorageObjectConnectionProvider storageObjectConnectionProvider, Configuration configuration)
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

    public async Task<long> InsertBlock(Block block)
    {
        var collection = await Get();
        var update = Builders<Block>.Update
            .Set(x => x.BlockNumber, block.BlockNumber)
            .Set(x => x.Api, block.Api)
            .Set(x => x.Cursor, block.Cursor);

        var options = new FindOneAndUpdateOptions<Block>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        var updated = await collection.FindOneAndUpdateAsync(x => x.Network == block.Network && x.Api == block.Api, update, options);
        return updated.BlockNumber;
    }

    public async Task<long> InsertHorizonBlock(uint blockNumber, string cursor = "")
    {
        return await InsertBlock(new Block
        {
            Network = await configuration.StellarHorizon,
            Api = StellarSettings.HorizonApi,
            BlockNumber = blockNumber,
            Cursor = cursor
        });
    }

    public async Task<long> InsertSorobanBlock(uint blockNumber, string cursor = "")
    {
        return await InsertBlock(new Block
        {
            Network = await configuration.StellarRpc,
            Api = StellarSettings.SorobanApi,
            BlockNumber = blockNumber,
            Cursor = cursor
        });
    }

    public async Task<Block?> Get(string network, string api)
    {
        var collection = await Get();
        var block = await collection
            .Find(x => x.Network == network && x.Api == api)
            .SingleOrDefaultAsync();
        return block;
    }
}