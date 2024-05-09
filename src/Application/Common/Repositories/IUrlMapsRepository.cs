using Domain.Common;
using Domain.UrlMaps.Entities;
using Domain.UrlMaps.ValueObjects;

namespace Application.Common.Repositories;

public interface IUrlMapsRepository
{
    public Task<Result> CreateAsync(UrlMap urlMap, CancellationToken token = default);
    public Task<Result<string>> FindRedirectAsync(ShortLink validationResultValue, CancellationToken token = default);
}