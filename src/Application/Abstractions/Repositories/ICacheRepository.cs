using Domain.Common;

namespace Application.Abstractions.Repositories;

public interface ICacheRepository
{
    public Task<Result<T>> FetchAsync<T>(string shortLink, CancellationToken token = default) where T : class;
    public Task AddAsync<T>(string shortLink, T value, CancellationToken token = default) where T : class;
    public Task DeleteAsync(string shortLink, CancellationToken token = default);
}