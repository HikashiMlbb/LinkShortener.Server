using Domain.Common;

namespace Application.Common.Repositories;

public interface ICacheRepository
{
    public Task<Result<T>> FetchAsync<T>(string shortLink, CancellationToken token = default) where T : class;
    public Task AddAsync<T>(string shortLinkValue, T value, CancellationToken token = default) where T : class;
}