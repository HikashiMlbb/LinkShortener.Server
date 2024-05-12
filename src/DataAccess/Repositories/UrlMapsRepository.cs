using Application.Common.Repositories;
using Application.UrlMaps;
using Dapper;
using DataAccess.Contexts.Abstractions;
using Domain.Common;
using Domain.UrlMaps.Entities;
using Domain.UrlMaps.ValueObjects;

namespace DataAccess.Repositories;

public class UrlMapsRepository(IDapperContext context, IEventDispatcher eventDispatcher) : IUrlMapsRepository
{
    public async Task<Result> CreateAsync(UrlMap urlMap, CancellationToken token = default)
    {
        using var db = context.Create();
        db.Open();
        
        var existingShortLink = await db.QuerySingleOrDefaultAsync<string>(GetByShortLinkQuery, new
        {
            ShortLink = urlMap.ShortLink.Value
        });

        if (existingShortLink is not null)
        {
            return new Error("UrlMaps.AlreadyExists", "UrlMap with such short link already exists.");
        }
        
        
        await db.ExecuteAsync(CreateQuery, new
        {
            ShortLink = urlMap.ShortLink.Value,
            RedirectLink = urlMap.RedirectLink.Value,
            urlMap.ExpiryDate,
            urlMap.LastVisitDate
        });
        
        return Result.Success();
    }

    public async Task<Result<RedirectLink>> FindRedirectAsync(ShortLink shortLink, CancellationToken token = default)
    {
        using var db = context.Create();
        db.Open();

        var result = await db.QuerySingleOrDefaultAsync<string>(GetByShortLinkQuery, new
        {
            ShortLink = shortLink.Value
        });

        return result is null
            ? new Error("UrlMaps.NotFound", "UrlMap with such short link has not been found.")
            : RedirectLink.Create(result);
    }

    public async Task<Result<IEnumerable<ShortLink>>> DeleteExpiredAsync(CancellationToken token = default)
    {
        using var db = context.Create();
        db.Open();

        var expiredShortLinks = (await db.QueryAsync<string>(GetExpiredQuery, new
        {
            Now = DateTime.UtcNow
        })).ToList();
        
        if (expiredShortLinks.Count == 0)
        {
            return new Error("UrlMaps.ExpiredNotFound", "Expired UrlMaps have not been found.");
        }

        await db.ExecuteAsync(DeleteExpiredQuery, new
        {
            List = expiredShortLinks 
        });

        var result = expiredShortLinks.Select(x => ShortLink.Create(x).Value!).ToList();

        await eventDispatcher.Raise(new ExpiredUrlMapsDeletedEvent(result), token);
        
        return Result<IEnumerable<ShortLink>>.Success(result);
    }

    #region Constraints of sql queries

    private const string GetByShortLinkQuery = $"""
                                                SELECT {nameof(UrlMap.ShortLink)} FROM {UrlMap.Plural}
                                                WHERE {nameof(UrlMap.ShortLink)}=@ShortLink
                                                """;

    private const string CreateQuery = $"""
                                        INSERT INTO {UrlMap.Plural} ({nameof(UrlMap.ShortLink)}, {nameof(UrlMap.RedirectLink)}, {nameof(UrlMap.ExpiryDate)}, {nameof(UrlMap.LastVisitDate)})
                                        VALUES (@ShortLink, @RedirectLink, @ExpiryDate, @LastVisitDate)
                                        """;

    private const string GetExpiredQuery = $"""
                                            SELECT {nameof(UrlMap.ShortLink)}
                                            FROM {UrlMap.Plural}
                                            WHERE
                                                ({nameof(UrlMap.ExpiryDate)} IS NOT NULL AND {nameof(UrlMap.ExpiryDate)} <= @Now)
                                                OR
                                                ({nameof(UrlMap.ExpiryDate)} IS NULL AND DATEDIFF(month, {nameof(UrlMap.LastVisitDate)}, @Now) >= 1)
                                            """;

    private const string DeleteExpiredQuery = $"""
                                               DELETE FROM {UrlMap.Plural}
                                               WHERE {nameof(UrlMap.ShortLink)} in @List
                                               """;
    #endregion
}