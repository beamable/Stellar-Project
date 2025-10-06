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
                }),
            new CreateIndexModel<Models.ExternalAuth>(Builders<Models.ExternalAuth>.IndexKeys
                .Ascending(x => x.Address)
                .Ascending(x => x.Message))
        ]);
        return _collection;
    }

    public async Task<Models.ExternalAuth?> Get(string address)
    {
        var collection = await Get();
        return await collection.Find(x => x.Address == address).FirstOrDefaultAsync();
    }

    public async Task<Models.ExternalAuth?> Get(string address, string message)
    {
        var collection = await Get();
        return await collection.Find(x => x.Address == address && x.Message == message).FirstOrDefaultAsync();
    }

    public async Task Upsert(Models.ExternalAuth vault)
    {
        var collection = await Get();
        try
        {
            var filter = Builders<Models.ExternalAuth>.Filter.Eq(x => x.Address, vault.Address);
            var options = new ReplaceOptions { IsUpsert = true };
            await collection.ReplaceOneAsync(filter, vault, options);
        }
        catch (Exception)
        {
            // ignored
        }
    }

}