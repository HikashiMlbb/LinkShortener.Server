using Domain.UrlMaps.ValueObjects;

namespace Application.Services.Interfaces;

public interface IShortLinkGenerator
{
    public ShortLink Generate();
}