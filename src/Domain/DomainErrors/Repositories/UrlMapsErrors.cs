using Domain.Common;

namespace Domain.DomainErrors.Repositories;

public static class UrlMapsErrors
{
    public static readonly Error AlreadyExists =
        new("UrlMaps.AlreadyExists", "UrlMap with such short link already exists.");

    public static readonly Error NotFound = new("UrlMaps.NotFound", "UrlMap with such short link has not been found.");

    public static readonly Error ExpiredNotFound =
        new("UrlMaps.ExpiredNotFound", "Expired UrlMaps have not been found.");
}