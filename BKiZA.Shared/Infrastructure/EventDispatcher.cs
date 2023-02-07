using System;
using Microsoft.Extensions.DependencyInjection;

namespace BKiZA.Shared.Infrastructure;

public class EventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Publish<TEvent>(string nodeId, TEvent @event) where TEvent : class, IEvent
    {
        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        var handlers = scope.ServiceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            handlerType
                .GetMethod(nameof(IEventHandler<TEvent>.Handle))?
                .Invoke(handler, new object[] { nodeId, @event });
        }
    }
}