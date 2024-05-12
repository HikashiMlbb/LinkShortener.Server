using Domain.UrlMaps.ValueObjects;

namespace Domain.UrlMaps.Entities;

public sealed class UrlMap
{
    public const string Plural = "UrlMaps";
    public ShortLink ShortLink { get; set; }
    public RedirectLink RedirectLink { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? LastVisitDate { get; private set; }

    public UrlMap(
        ShortLink shortLink, 
        RedirectLink redirectLink, 
        DateTime? expiryDate = null,
        DateTime? lastVisitDate = null)
    {
        ShortLink = shortLink;
        RedirectLink = redirectLink;
        ExpiryDate = expiryDate;
        LastVisitDate = lastVisitDate ?? DateTime.UtcNow;
    }
}