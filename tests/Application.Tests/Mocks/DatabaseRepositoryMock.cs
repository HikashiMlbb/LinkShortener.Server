using Application.Abstractions.Repositories;
using Domain.Common;
using Domain.UrlMaps.Entities;
using Domain.UrlMaps.ValueObjects;

namespace Application.Tests.Mocks;

public class DatabaseRepositoryMock : IUrlMapsRepository
{
    private readonly List<UrlMap> _urlMaps = [];
    public async Task<Result> CreateAsync(UrlMap urlMap, CancellationToken token = default)
    {
        return await Task.Run(() =>
        {
            _urlMaps.Add(urlMap);
            return Result.Success();
        }, token);
    }

    public async Task<Result<RedirectLink>> FindRedirectAsync(ShortLink shortLink, CancellationToken token = default)
    {
        var func = new Func<Result<RedirectLink>>(() =>
        {
            var result = _urlMaps.Find(x => x.ShortLink == shortLink);
            return result is null
                ? new Error("NotFound")
                : result.RedirectLink;
        });
        
        return await Task.Run(func, token);
    }

    public async Task<Result<IEnumerable<ShortLink>>> DeleteExpiredAsync(CancellationToken token = default)
    {
        return await Task.Run(GetExpiredShortLinks, token);
    }

    private Result<IEnumerable<ShortLink>> GetExpiredShortLinks()
    {
        var expired = _urlMaps.Select(x => x.ShortLink);
        _urlMaps.RemoveAll(_ => true);
        return Result<IEnumerable<ShortLink>>.Success(expired);
    }
}