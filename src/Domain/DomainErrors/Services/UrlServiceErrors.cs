using Domain.Common;

namespace Domain.DomainErrors.Services;

public static class UrlServiceErrors
{
    public static readonly Error NotFound = new("UrlService.NotFound", "Given short link has not been found.");

    public static readonly Error CreateError =
        new("UrlService.CreateFailure", "Something went wrong during creating short link.");
}