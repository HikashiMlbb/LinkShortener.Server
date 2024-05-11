using Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Tests.TestEvents;

public class TestEventHandler(IServiceProvider provider) : IEventHandler<TestEvent>
{
    public Task HandleAsync(TestEvent @event, CancellationToken token = default)
    {
        provider.GetRequiredService<IEventDispatcher>();
        return Task.CompletedTask;
    }
}