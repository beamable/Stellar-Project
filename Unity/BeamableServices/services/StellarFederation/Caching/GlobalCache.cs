using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Microsoft.Extensions.Caching.Memory;

namespace Beamable.StellarFederation.Caching;

public static class GlobalCache
{
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions());
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(10);

    public static async Task<T?> GetOrCreate<T>(
        string key,
        Func<ICacheEntry, Task<T?>> factory,
        TimeSpan? expiration = null) where T : class
    {
        var value = await Cache.GetOrCreateAsync(key, entry =>
        {
            try
            {
                entry.AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration;
                return factory(entry);
            }
            catch (Exception ex)
            {
                BeamableLogger.LogWarning($"Resolving cache entry for '{key}' threw an exception.", ex);
                throw;
            }
        });

        if (value is null)
            Cache.Remove(key);
        return value;
    }

    public static async Task<T?> GetOrCreate<T>(
        string key,
        Func<ICacheEntry, Task<T?>> factory,
        TimeSpan? expiration = null) where T : struct
    {
        var value = await Cache.GetOrCreateAsync(key, entry =>
        {
            try
            {
                entry.AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration;
                return factory(entry);
            }
            catch (Exception ex)
            {
                BeamableLogger.LogWarning($"Resolving cache entry for '{key}' threw an exception.", ex);
                throw;
            }
        });

        if (!value.HasValue)
            Cache.Remove(key);
        return value;
    }

    public static void Invalidate(string key)
    {
        try
        {
            Cache.Remove(key);
        }
        catch (Exception ex)
        {
            BeamableLogger.LogWarning($"Disposing cache entry for '{key}' threw an exception.", ex);
        }
    }

    public static void InvalidateAccountCache(IEnumerable<string> keys)
    {
        try
        {
            foreach (var key in keys)
            {
                Invalidate($"account-{key}");
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}