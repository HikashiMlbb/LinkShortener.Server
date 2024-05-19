using Application.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddEventHandler<TEvent, TEventHandler>(this IServiceCollection services)
        where TEvent : IEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var serviceType = typeof(IEventHandler<>).MakeGenericType(typeof(TEvent));
        var implementationType = typeof(TEventHandler);
        return services.AddScoped(serviceType, implementationType);
    }
}