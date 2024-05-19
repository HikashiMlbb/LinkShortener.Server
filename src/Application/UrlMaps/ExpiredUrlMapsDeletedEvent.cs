using Application.Abstractions.Events;
using Domain.UrlMaps.ValueObjects;

namespace Application.UrlMaps;

public record ExpiredUrlMapsDeletedEvent(IEnumerable<ShortLink> ShortLinks) : IEvent;