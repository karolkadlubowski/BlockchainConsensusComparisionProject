using BKiZA.Shared.Infrastructure;

namespace BKiZA.Shared.Network;

public class NetworkBroker
{
    private readonly NetworkEventChannel _networkEventChannel;

    public NetworkBroker(NetworkEventChannel networkEventChannel)
    {
        _networkEventChannel = networkEventChannel;
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : class, IEvent
    {
        if (@event is null)
        {
            return;
        }

        _networkEventChannel.Writer.TryWrite(@event);
    }
}