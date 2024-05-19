using Application.Common.Events;
using Application.Common.Repositories;
using Application.Extensions;
using Application.Services;
using Application.Services.Interfaces;
using Application.UrlMaps;
using DataAccess.Contexts;
using DataAccess.Contexts.Abstractions;
using DataAccess.Migrations;
using DataAccess.Options;
using DataAccess.Repositories;
using Domain.Common;
using Domain.UrlMaps.Entities;
using Domain.UrlMaps.ValueObjects;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataAccess.Tests;

public class ContextTests
{
    [Fact]
    public async Task CreateAndDeleteExpired()
    {
        var provider = GetServiceProvider();

        var databaseManager = provider.GetRequiredService<DatabaseManager>();
        var repo = provider.GetRequiredService<IUrlMapsRepository>();
        var generator = provider.GetRequiredService<IShortLinkGenerator>();
        var options = provider.GetRequiredService<IOptions<DataAccessOptions>>();
        var migrationManager = provider.GetRequiredService<IMigrationRunner>();
        
        databaseManager.Create(options.Value.Database);
        
        migrationManager.ListMigrations();
        migrationManager.MigrateUp();

        List<UrlMap> urlMaps = [
            new UrlMap(generator.Generate(), RedirectLink.Create("https://not.www.youtube.com/null-expiry").Value!),
            new UrlMap(generator.Generate(), RedirectLink.Create("https://not.www.youtube.com/alr-expired").Value!, DateTime.UtcNow.Subtract(TimeSpan.FromDays(60))),
            new UrlMap(generator.Generate(), RedirectLink.Create("https://not.www.youtube.com/null-expiry-but-forgot").Value!, null, DateTime.UtcNow.Subtract(TimeSpan.FromDays(60)))
        ];
        
        await Task.WhenAll(urlMaps.Select(x => repo.CreateAsync(x, CancellationToken.None)));
        
        var result = await repo.DeleteExpiredAsync();
        
        Assert.True(result.IsSuccess);
    }

    private static ServiceProvider GetServiceProvider()
    {
        DotNetEnv.Env.Load();
        
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets("5e3cf4cd-dd14-4f13-8e97-a869eb3172aa")
            .Build();

        var services = new ServiceCollection();

        services.AddScoped<IDapperContext, DapperContext>();
        services.AddSingleton<DatabaseManager>();
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

        services.AddFluentMigratorCore()
            .ConfigureRunner(x => x
                .AddSqlServer()
                .WithGlobalConnectionString("Server=localhost;Database=TestDatabase;User Id=sa;Password=saroot1234$;TrustServerCertificate=True")
                .WithMigrationsIn(typeof(InitialCreate).Assembly));

        return services.BuildServiceProvider();
    }
}