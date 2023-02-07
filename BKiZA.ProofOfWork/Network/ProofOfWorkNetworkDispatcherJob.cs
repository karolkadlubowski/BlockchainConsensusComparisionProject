using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BKiZA.ProofOfWork.Nodes;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BKiZA.ProofOfWork.Network;

public class ProofOfWorkNetworkDispatcherJob : BackgroundService
{
    private readonly ILogger<ProofOfWorkNetworkDispatcherJob> _logger;
    private readonly NetworkEventChannel _networkEventChannel;
    private readonly EventDispatcher _eventDispatcher;
    private readonly INetworkStorage<Miner> _minerNetworkStorage;

    public ProofOfWorkNetworkDispatcherJob(ILogger<ProofOfWorkNetworkDispatcherJob> logger,
        NetworkEventChannel networkEventChannel,
        EventDispatcher eventDispatcher,
        INetworkStorage<Miner> minerNetworkStorage)
    {
        _logger = logger;
        _networkEventChannel = networkEventChannel;
        _eventDispatcher = eventDispatcher;
        _minerNetworkStorage = minerNetworkStorage;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (await _networkEventChannel.Reader.WaitToReadAsync(cancellationToken))
            {
                while (_networkEventChannel.Reader.TryRead(out var @event))
                {
                    _eventDispatcher.Publish("A", @event);
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
        }
    }
}