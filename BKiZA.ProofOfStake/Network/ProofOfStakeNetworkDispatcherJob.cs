using System;
using System.Threading;
using System.Threading.Tasks;
using BKiZA.ProofOfStake.Nodes;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BKiZA.ProofOfStake.Network;

public class ProofOfStakeNetworkDispatcherJob : BackgroundService
{
    private readonly ILogger<ProofOfStakeNetworkDispatcherJob> _logger;
    private readonly NetworkEventChannel _networkEventChannel;
    private readonly EventDispatcher _eventDispatcher;
    private readonly INetworkStorage<Validator> _validatorNetworkStorage;

    public ProofOfStakeNetworkDispatcherJob(ILogger<ProofOfStakeNetworkDispatcherJob> logger,
        NetworkEventChannel networkEventChannel,
        EventDispatcher eventDispatcher,
        INetworkStorage<Validator> validatorNetworkStorage)
    {
        _logger = logger;
        _networkEventChannel = networkEventChannel;
        _eventDispatcher = eventDispatcher;
        _validatorNetworkStorage = validatorNetworkStorage;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (await _networkEventChannel.Reader.WaitToReadAsync(cancellationToken))
            {
                while (_networkEventChannel.Reader.TryRead(out var @event))
                {
                    var validators = _validatorNetworkStorage.Scan();

                    foreach (var validator in validators)
                    {
                        _eventDispatcher.Publish(validator.NodeId, @event);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
        }
    }
}