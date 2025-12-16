using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beamable.StellarFederation.Extensions;

public static class EnumerableExtensions
{
    private static readonly Random Random = new();

    public static IEnumerable<string> Shuffle(this IEnumerable<string> source)
    {
        // Fisher-Yates shuffle algorithm
        var buffer = source.ToArray();
        if (buffer.Length == 1) return buffer;
        for (var i = buffer.Length - 1; i > 0; i--)
        {
            var j = Random.Next(i + 1);
            (buffer[i], buffer[j]) = (buffer[j], buffer[i]);
        }
        return buffer;
    }

    public static int CountFast<T>(this IEnumerable<T> source)
      => source.TryGetNonEnumeratedCount(out var count) ? count : source.Count();

    public static bool IsNullOrEmpty<T>(this List<T>? list)
        => list == null || list.Count == 0;

    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        var list = new List<T>();

        await foreach (var item in source)
        {
            list.Add(item);
        }

        return list;
    }

}