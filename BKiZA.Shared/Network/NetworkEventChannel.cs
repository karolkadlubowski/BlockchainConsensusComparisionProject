using System.Threading.Channels;
using BKiZA.Shared.Infrastructure;

namespace BKiZA.Shared.Network;

public class NetworkEventChannel
{
    private readonly Channel<IEvent> _channel = Channel.CreateUnbounded<IEvent>();

    public ChannelWriter<IEvent> Writer => _channel.Writer;
    public ChannelReader<IEvent> Reader => _channel.Reader;
}