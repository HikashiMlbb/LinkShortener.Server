using System.Text.Json;
using Application.Common.Repositories;
using Domain.Common;
using Microsoft.Extensions.Caching.Distributed;

namespace DataAccess.Repositories;

public class CacheRepository(IDistributedCache cache) : ICacheRepository
{
    public async Task<Result<T>> FetchAsync<T>(string shortLink, CancellationToken token = default) where T : class
    {
        var value = await cache.GetStringAsync(shortLink, token);

        if (value is null)
        {
            return new Error("Cache.NotFound", "There is no value with such key in cache.");
        }

        var result = JsonSerializer.Deserialize<T>(value);

        return result is null
            ? new Error("Cache.MappingFailure", $"Unable to convert string to {typeof(T)}")
            : result;
    }

    public async Task AddAsync<T>(string shortLink, T value, CancellationToken token = default) where T : class
    {
        var serializedObject = JsonSerializer.Serialize(value);

        await cache.SetStringAsync(shortLink, serializedObject, token);
    }

    public async Task DeleteAsync(string shortLink, CancellationToken token = default)
    {
        await cache.RemoveAsync(shortLink, token);
    }
}