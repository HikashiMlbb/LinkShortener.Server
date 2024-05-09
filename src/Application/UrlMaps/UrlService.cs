using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Common;
using Domain.UrlMaps.Entities;
using Domain.UrlMaps.ValueObjects;

namespace Application.UrlMaps;

public sealed class UrlService : IUrlService
{
    private readonly IShortLinkGenerator _generator;
    private readonly ICacheRepository _cache;
    private readonly IUrlMapsRepository _urlMapsRepo;

    public UrlService(IShortLinkGenerator generator, ICacheRepository cache, IUrlMapsRepository urlMapsRepo)
    {
        _generator = generator;
        _cache = cache;
        _urlMapsRepo = urlMapsRepo;
    }
    
    public async Task<Result<string>> FetchRedirectLinkAsync(string shortLink, CancellationToken token = default)
    {
        var validationResult = ShortLink.Create(shortLink);
        if (validationResult.IsFailure)
        {
            return validationResult.Error!;
        }

        var fromCacheResult = await _cache.FetchAsync<string>(shortLink);

        if (fromCacheResult.IsSuccess)
        {
            return fromCacheResult.Value!;
        }

        var fromDbResult = await _urlMapsRepo.FindRedirectAsync(validationResult.Value);

        if (fromDbResult.IsSuccess)
        {
            return fromDbResult.Value!;
        }

        return new Error("UrlService.NotFound", "Given short link has not been found.");
    }

    public async Task<Result<string>> CreateShortLinkAsync(string redirectLink, TimeSpan? expiryTimeout = null, CancellationToken token = default)
    {
        var validationResult = RedirectLink.Create(redirectLink);
        if (validationResult.IsFailure)
        {
            return validationResult.Error!;
        }

        var attemptsLeft = 10;
        var isSuccess = false;
        var result = string.Empty;

        while (!isSuccess && attemptsLeft >= 0)
        {
            var shortLink = _generator.Generate();
            var newRedirectLink = validationResult.Value!;

            result = shortLink.Value;
            attemptsLeft--;

            var urlMap = new UrlMap(
                shortLink, 
                newRedirectLink,
                expiryTimeout is null ? null : DateOnly.FromDateTime(DateTime.UtcNow).AddDays((int)expiryTimeout.Value.TotalDays));

            isSuccess = (await _urlMapsRepo.CreateAsync(urlMap, token)).IsSuccess;
            await _cache.AddAsync(shortLink.Value, newRedirectLink.Value, token);
        }

        return !isSuccess
            ? new Error("UrlService.CreateError", "Something went wrong during creating short link.")
            : result;
    }
}