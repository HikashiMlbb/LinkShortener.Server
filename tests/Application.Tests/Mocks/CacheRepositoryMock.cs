using System.Text.Json;
using Application.Abstractions.Repositories;
using Domain.Common;

namespace Application.Tests.Mocks;

public class CacheRepositoryMock : ICacheRepository
{
    private readonly Dictionary<string, string> _cache = new();
    
    public async Task<Result<T>> FetchAsync<T>(string shortLink, CancellationToken token = default) where T : class
    {
        return await Task.Run(new Func<Result<T>>(() =>
        {
            var val = _cache.ContainsKey(shortLink);
            
            if (_cache.TryGetValue(shortLink, out var cacheValue))
            {
                return new Error("NotFound");
            }
            
            var res = JsonSerializer.Deserialize<T>(cacheValue!)!;
            

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