using Application.Common.Errors;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Common;
using Domain.UrlMaps.Entities;
using Domain.UrlMaps.ValueObjects;

namespace Application.UrlMaps;

public sealed class UrlService(
    IShortLinkGenerator generator, 
    ICacheRepository cache, 
    IUrlMapsRepository urlMapsRepo)
    : IUrlService
{
    // TODO: Create a Result Combine.
    public async Task<Result<string>> FetchRedirectLinkAsync(string shortLink, CancellationToken token = default)
    {
        var validationResult = ShortLink.Create(shortLink);
        if (validationResult.IsFailure)
        {
            return validationResult.Error!;
        }

        var fromCacheResult = await cache.FetchAsync<string>(shortLink, token);

        if (fromCacheResult.IsSuccess)
        {
            return fromCacheResult.Value!;
        }

        var fromDbResult = await urlMapsRepo.FindRedirectAsync(validationResult.Value!, token);

        if (fromDbResult.IsSuccess)
        {
            return fromDbResult.Value!.Value;
        }

        return UrlServiceErrors.NotFound;
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
            var shortLink = generator.Generate();
            var newRedirectLink = validationResult.Value!;

            result = shortLink.Value;
            attemptsLeft--;

            var urlMap = new UrlMap(
                shortLink, 
                newRedirectLink,
                expiryTimeout is null ? null : DateOnly.FromDateTime(DateTime.UtcNow).AddDays((int)expiryTimeout.Value.TotalDays));

            isSuccess = (await urlMapsRepo.CreateAsync(urlMap, token)).IsSuccess;
            await cache.AddAsync(shortLink.Value, newRedirectLink.Value, token);
        }

        return !isSuccess
            ? UrlServiceErrors.CreateError
            : result;
    }
}