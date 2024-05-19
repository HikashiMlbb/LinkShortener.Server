using Application.Abstractions.Events;
using Application.Abstractions.Repositories;

namespace Application.UrlMaps;

// ReSharper disable once ClassNeverInstantiated.Global
public class ExpiredUrlMapsDeletedEventHandler(ICacheRepository repo) : IEventHandler<ExpiredUrlMapsDeletedEvent>
{
    public async Task HandleAsync(ExpiredUrlMapsDeletedEvent @event, CancellationToken token = default)
    {
        var links = @event.ShortLinks;

        await Task.WhenAll(links.Select(link => repo.DeleteAsync(link.Value, token)).ToArray());
    }
}