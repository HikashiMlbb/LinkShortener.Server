using System.Text.Json;
using Application.Abstractions.Repositories;
using DataAccess.Options;
using Domain.Common;
using Domain.DomainErrors.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DataAccess.Repositories;

public class CacheRepository(IDistributedCache cache, IOptions<DataAccessOptions> options) : ICacheRepository
{
    private readonly TimeSpan? _cacheTimeout = options.Value.CacheTimeout;

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

        await cache.SetStringAsync(shortLink, serializedObject, new DistributedCacheEntryOptions
        {
            SlidingExpiration = _cacheTimeout
        }, token);
    }

    public async Task DeleteAsync(string shortLink, CancellationToken token = default)
    {
        await cache.RemoveAsync(shortLink, token);
    }
}