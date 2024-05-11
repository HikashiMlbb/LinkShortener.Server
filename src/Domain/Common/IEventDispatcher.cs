namespace Domain.Common;

public interface IEventDispatcher
{
    public Task Raise<TEvent>(TEvent @event) where TEvent : IEvent;
}