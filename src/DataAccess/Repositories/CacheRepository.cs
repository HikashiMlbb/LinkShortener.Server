using System.Text.Json;
using Application.Common.Repositories;
using Domain.Common;
using Domain.DomainErrors.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace DataAccess.Repositories;

public class CacheRepository(IDistributedCache cache) : ICacheRepository
{
    public async Task<Result<T>> FetchAsync<T>(string shortLink, CancellationToken token = default) where T : class
    {
        var value = await cache.GetStringAsync(shortLink, token);

        if (value is null) return CacheErrors.NotFound;

        var result = JsonSerializer.Deserialize<T>(value);

        return result is null
            ? CacheErrors.DeserializeFailure
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