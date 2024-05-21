using API.DTOs;
using Application.Abstractions.Events;
using Application.Abstractions.Repositories;
using Application.Events;
using Application.Extensions;
using Application.Services;
using Application.Services.Interfaces;
using Application.UrlMaps;
using DataAccess;
using DataAccess.Contexts;
using DataAccess.Contexts.Abstractions;
using DataAccess.Options;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// --- Application Layer Adding ---
builder.Services.AddScoped<IShortLinkGenerator, ShortLinkGenerator>();
builder.Services.AddScoped<IUrlService, UrlService>();

// --- Event Adding ---
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddEventHandler<ExpiredUrlMapsDeletedEvent, ExpiredUrlMapsDeletedEventHandler>();

// --- DataAccess Layer Adding ---
builder.Services.AddScoped<ICacheRepository, CacheRepository>();
builder.Services.AddScoped<IUrlMapsRepository, UrlMapsRepository>();
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<DatabaseManager>();

// --- Options Patter Adding ---
builder.Services.Configure<DataAccessOptions>(
    builder.Configuration.GetSection(DataAccessOptions.DataAccess));

// --- Cache Adding ---
builder.Services.AddStackExchangeRedisCache(x =>
{
    x.Configuration = builder.Configuration.GetValue<string>("DATAACCESS:CACHECONFIGURATION") 
                      ?? throw new ApplicationException("Unable to start application. DATAACCESS:CACHECONFIGURATION is empty or not found.");

    x.InstanceName = builder.Configuration.GetValue<string>("DATAACCESS:CACHEINSTANCE") ?? "linkshortener_";
});


var app = builder.Build();

app.MapGet("/api/redirect", async (
    [FromQuery]string link, 
    [FromServices]IUrlService service) =>
{
    var result = await service.FetchRedirectLinkAsync(link);

    return result.IsFailure 
        ? Results.NotFound(result.Error) 
        : Results.Ok(result.Value);
});

app.MapPost("/api/shorten", async (
    [FromBody] ShortenBodyDto body,
    [FromServices] IUrlService service) =>
{
    var result = await service.CreateShortLinkAsync(body.Link!, body.Expiration);

    return result.IsFailure
        ? Results.BadRequest(result.Error)
        : Results.Ok(result.Value);
});

app.Run();