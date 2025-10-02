using System;
using System.Threading.Tasks;
using Beamable.Server;
using MongoDB.Driver;

namespace Beamable.StellarFederation.Features.ExternalAuth.Storage;

public class ExternalAuthCollection(IStorageObjectConnectionProvider storageObjectConnectionProvider)
    : IService
{
    private IMongoCollection<Models.ExternalAuth>? _collection;

    private async ValueTask<IMongoCollection<Models.ExternalAuth>> Get()
    {
        if (_collection is not null) return _collection;
        _collection =
            (await storageObjectConnectionProvider.StellarFederationStorageDatabase()).GetCollection<Models.ExternalAuth>("external_auth");
        await _collection.Indexes.CreateManyAsync([
            new CreateIndexModel<Models.ExternalAuth>(Builders<Models.ExternalAuth>.IndexKeys.Ascending(x => x.ExpiresAt),
                new CreateIndexOptions
                {
                    ExpireAfter = TimeSpan.Zero
                })
        ]);
        return _collection;
    }

    public async Task<Models.ExternalAuth?> Get(string message)
    {
        var collection = await Get();
        return await collection.Find(x => x.Message == message).FirstOrDefaultAsync();
    }

    public async Task<bool> TryInsert(Models.ExternalAuth vault)
    {
        var collection = await Get();
        try
        {
            await collection.InsertOneAsync(vault);
            return true;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return false;
        }
    }
}