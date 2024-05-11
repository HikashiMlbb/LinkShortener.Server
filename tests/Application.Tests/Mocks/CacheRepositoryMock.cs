using System.Text.Json;
using Application.Common.Repositories;
using Domain.Common;

namespace Application.Tests.Mocks;

public class CacheRepositoryMock : ICacheRepository
{
    private readonly Dictionary<string, string> _cache = new();
    
    public async Task<Result<T>> FetchAsync<T>(string shortLink, CancellationToken token = default) where T : class
    {
        return await Task.Run(new Func<Result<T>>(() =>
        {
            var val = _cache[shortLink];
            var res = JsonSerializer.Deserialize<T>(val);

            if (res is null)
            {
                return new Error("NotFound");
            }

            return res;
        }), token);
    }

    public async Task AddAsync<T>(string shortLink, T value, CancellationToken token = default) where T : class
    {
        await Task.Run(() =>
        {
            var val = JsonSerializer.Serialize(value);
            _cache.Add(shortLink, val);
        }, token);
    }

    public async Task DeleteAsync(string shortLink, CancellationToken token = default)
    {
        await Task.Run(() => _cache.Remove(shortLink), token);
    }
}