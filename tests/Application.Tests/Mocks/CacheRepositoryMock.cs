using System.Text.Json;
using Application.Common.Repositories;
using Domain.Common;

namespace Application.Tests.Mocks;

public class CacheRepositoryMock : ICacheRepository
{
    private readonly Dictionary<string, string> _cache = new();
    
    public async Task<Result<T>> FetchAsync<T>(string shortLink, CancellationToken token = default) where T : class
    {
        var val = _cache[shortLink];
        var res = JsonSerializer.Deserialize<T>(val);

        if (res is null)
        {
            return new Error("NotFound");
        }

        return res;
    }

    public async Task AddAsync<T>(string shortLinkValue, T value, CancellationToken token = default) where T : class
    {
        var val = JsonSerializer.Serialize(value);
        _cache.Add(shortLinkValue, val);
    }
}