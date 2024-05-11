using System.Collections.Concurrent;
using Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Events;

public class EventDispatcher(IServiceProvider provider) : IEventDispatcher
{
    public Task Raise<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var type = typeof(IEventHandler<>).MakeGenericType(typeof(TEvent));
        var services = provider.GetServices(type);
        var tasks = new List<Task>();

        foreach (var service in services)
        {
            if ((service as IEventHandler<TEvent>) is not { } obj)
            {
                continue;
            }

            tasks.Add(obj.HandleAsync(@event));
        }

        Task.WaitAll(tasks.ToArray());
        return Task.CompletedTask;
    }
}