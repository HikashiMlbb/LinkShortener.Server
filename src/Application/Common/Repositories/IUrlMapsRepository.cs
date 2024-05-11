using Domain.Common;
using Domain.UrlMaps.Entities;
using Domain.UrlMaps.ValueObjects;

namespace Application.Common.Repositories;

public interface IUrlMapsRepository
{
    /// <summary>
    ///     Creates the record in database.
    /// </summary>
    /// <param name="urlMap">The <see cref="UrlMap"/> entity.</param>
    /// <param name="token">An <see cref="CancellationToken"/> token.</param>
    /// <returns>Asynchronous task with <see cref="Result"/> of execution.</returns>
    public Task<Result> CreateAsync(UrlMap urlMap, CancellationToken token = default);

    /// <summary>
    ///     Searches the record in database.
    /// </summary>
    /// <param name="shortLink">The <see cref="ShortLink"/> DTO of record.</param>
    /// <param name="token">An <see cref="CancellationToken"/> token.</param>
    /// <returns>Asynchronous task with <see cref="Result"/> that contains RedirectLink</returns>
    public Task<Result<RedirectLink>> FindRedirectAsync(ShortLink shortLink, CancellationToken token = default);
    /// <summary>
    ///     Deletes all expired records in database.
    /// </summary>
    /// <param name="token">An <see cref="CancellationToken"/> token.</param>
    /// <returns>Asynchronous task with <see cref="Result"/> that contains all <see cref="ShortLink"/> which are expired.</returns>
    public Task<Result<IEnumerable<ShortLink>>> DeleteExpiredAsync(CancellationToken token = default);
}