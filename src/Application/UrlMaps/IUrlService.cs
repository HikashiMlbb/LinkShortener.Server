using Domain.Common;

namespace Application.UrlMaps;

public interface IUrlService
{
    public Task<Result<string>> FetchRedirectLinkAsync(string shortLink, CancellationToken token = default);

    public Task<Result<string>> CreateShortLinkAsync(string redirectLink, TimeSpan? expiryTimeout = null,
        CancellationToken token = default);
}