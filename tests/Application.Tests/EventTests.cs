using System.Collections.Immutable;
using Application.Common.Events;
using Application.Common.Repositories;
using Application.Extensions;
using Application.Tests.Mocks;
using Application.Tests.TestEvents;
using Application.UrlMaps;
using Domain.Common;
using Domain.UrlMaps.Entities;
using Domain.UrlMaps.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Tests;

public class EventTests
{
    [Fact]
    public async Task EventRaisingTest()
    {
        var services = new ServiceCollection();

        services.AddEventHandler<TestEvent, TestEventHandler>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        var provider = services.BuildServiceProvider();

        var service = provider.GetRequiredService<IEventDispatcher>();

        await service.Raise(new TestEvent());
    }

    [Fact]
    public async Task RemovingExpiredCacheUsingEvent()
    {
        var services = new ServiceCollection();

        services.AddEventHandler<ExpiredUrlMapsDeletedEvent, ExpiredUrlMapsDeletedEventHandler>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddScoped<ICacheRepository, CacheRepositoryMock>();
        
        var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IEventDispatcher>();
        var repo = provider.GetRequiredService<ICacheRepository>();
        
        var list = new List<ShortLink>
        {
            ShortLink.Create("qwertyz").Value!,
            ShortLink.Create("zxcvbn").Value!,
            ShortLink.Create("asdfghj").Value!
        };

        await Task.WhenAll(list.Select(x => repo.AddAsync(x.Value, "some value")));
        
        await dispatcher.Raise(new ExpiredUrlMapsDeletedEvent(list));
    }

    [Fact]
    public async Task DatabaseCleanupBackgroundJobImitateTest()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddEventHandler<ExpiredUrlMapsDeletedEvent, ExpiredUrlMapsDeletedEventHandler>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddScoped<ICacheRepository, CacheRepositoryMock>();
        services.AddScoped<IUrlMapsRepository, DatabaseRepositoryMock>();
        
        var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IEventDispatcher>();
        var dbRepo = provider.GetRequiredService<IUrlMapsRepository>();
        var cache = provider.GetRequiredService<ICacheRepository>();
        
        var shortLinks = new List<ShortLink>
        {
            ShortLink.Create("qwertyz").Value!,
            ShortLink.Create("zxcvbn").Value!,
            ShortLink.Create("asdfghj").Value!
        };

        var urlMaps = new List<UrlMap>()
        {
            new(shortLinks[0], RedirectLink.Create("https://hello.world/").Value!),
            new(shortLinks[1], RedirectLink.Create("https://no.hello.world/").Value!),
            new(shortLinks[2], RedirectLink.Create("https://no.world/").Value!),
        };

        // Add to database
        await Task.WhenAll(urlMaps.Select(x => dbRepo.CreateAsync(x)));
        // Add to cache
        await Task.WhenAll(urlMaps.Select(x => cache.AddAsync(x.ShortLink.Value, x.RedirectLink.Value)));
        
        
        // Act
        var result = await dbRepo.DeleteExpiredAsync();
        await dispatcher.Raise(new ExpiredUrlMapsDeletedEvent(result.Value!));
        
        
        // Assert
        Assert.True((await cache.FetchAsync<string>(shortLinks.First().Value)).IsFailure);
    }
}