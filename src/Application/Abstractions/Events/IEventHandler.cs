namespace Application.Abstractions.Events;

public interface IEventHandler<TEvent> where TEvent : IEvent
{
    public Task HandleAsync(TEvent @event, CancellationToken token = default);
}