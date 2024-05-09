using Application.Common.Repositories;
using Domain.Common;
using Domain.UrlMaps.Entities;
using Domain.UrlMaps.ValueObjects;

namespace Application.Tests.Mocks;

public class DatabaseRepositoryMock : IUrlMapsRepository
{
    private readonly List<UrlMap> _urlMaps = [];
    public async Task<Result> CreateAsync(UrlMap urlMap, CancellationToken token = default)
    {
        _urlMaps.Add(urlMap);
        return Result.Success();
    }

    public async Task<Result<string>> FindRedirectAsync(ShortLink validationResultValue, CancellationToken token = default)
    {
        var result = _urlMaps.Find(x => x.ShortLink == validationResultValue);
        return result is null
            ? new Error("NotFound")
            : result.RedirectLink.Value;
    }
}