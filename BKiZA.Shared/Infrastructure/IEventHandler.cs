namespace BKiZA.Shared.Infrastructure;

public interface IEventHandler<in TEvent> where TEvent : class, IEvent
{
    void Handle(string nodeId, TEvent @event);
}