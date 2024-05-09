using Domain.UrlMaps.ValueObjects;

namespace Domain.UrlMaps.Entities;

public sealed class UrlMap
{
    public ShortLink ShortLink { get; set; }
    public RedirectLink RedirectLink { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public DateOnly? LastVisitDate { get; private set; }

    public UrlMap(
        ShortLink shortLink, 
        RedirectLink redirectLink, 
        DateOnly? expiryDate = null,
        DateOnly? lastVisitDate = null)
    {
        ShortLink = shortLink;
        RedirectLink = redirectLink;
        ExpiryDate = expiryDate;
        LastVisitDate = lastVisitDate;
    }

    public void UpdateLastVisit()
    {
        LastVisitDate = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}