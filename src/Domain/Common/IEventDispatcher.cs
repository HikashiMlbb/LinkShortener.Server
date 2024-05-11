namespace Domain.Common;

public interface IEventDispatcher
{
    public Task Raise<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent;
}