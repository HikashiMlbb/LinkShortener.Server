using Application.Common.Events;
using Application.Common.Repositories;
using Application.Extensions;
using Application.Services;
using Application.Services.Interfaces;
using Application.UrlMaps;
using DataAccess.Contexts;
using DataAccess.Contexts.Abstractions;
using DataAccess.Options;
using DataAccess.Repositories;
using Domain.Common;
using Domain.UrlMaps.Entities;
using Domain.UrlMaps.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Tests;

public class ContextTests
{
    [Fact]
    public async Task CreateAndDeleteExpired()
    {
        DotNetEnv.Env.Load();
        
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets("5e3cf4cd-dd14-4f13-8e97-a869eb3172aa")
            .Build();

        var services = new ServiceCollection();

        services.AddScoped<IDapperContext, DapperContext>();
        services.AddSingleton<IConfiguration>(config);
        services.Configure<DataAccessOptions>(config.GetSection(DataAccessOptions.DataAccess));
        services.AddScoped<IUrlMapsRepository, UrlMapsRepository>();
        services.AddScoped<IShortLinkGenerator, ShortLinkGenerator>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddEventHandler<ExpiredUrlMapsDeletedEvent, ExpiredUrlMapsDeletedEventHandler>();
        services.AddScoped<ICacheRepository, CacheRepository>();
        services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = "localhost";
            opt.InstanceName = "test";
        });

        var provider = services.BuildServiceProvider();

        var repo = provider.GetRequiredService<IUrlMapsRepository>();
        var generator = provider.GetRequiredService<IShortLinkGenerator>();

        List<UrlMap> urlMaps = [
            new UrlMap(generator.Generate(), RedirectLink.Create("https://not.www.youtube.com/null-expiry").Value!),
            new UrlMap(generator.Generate(), RedirectLink.Create("https://not.www.youtube.com/alr-expired").Value!, DateTime.UtcNow.Subtract(TimeSpan.FromDays(60))),
            new UrlMap(generator.Generate(), RedirectLink.Create("https://not.www.youtube.com/null-expiry-but-forgot").Value!, null, DateTime.UtcNow.Subtract(TimeSpan.FromDays(60)))
        ];

        await Task.WhenAll(urlMaps.Select(x => repo.CreateAsync(x, CancellationToken.None)));
        
        var result = await repo.DeleteExpiredAsync();
        
        Assert.True(result.IsSuccess);
    }
}