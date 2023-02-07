using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;

namespace BKiZA.ProofOfStake.Nodes.Events.Handlers;

public class ValidatorBlockDiggedHandler : IEventHandler<ValidatorBlockDigged>
{
    private readonly INetworkStorage<Validator> _networkStorage;
    private readonly NetworkBroker _networkBroker;

    public ValidatorBlockDiggedHandler(INetworkStorage<Validator> networkStorage,
        NetworkBroker networkBroker)
    {
        _networkStorage = networkStorage;
        _networkBroker = networkBroker;
    }
    public void Handle(string nodeId, ValidatorBlockDigged @event)
    {
        var currentNode = _networkStorage.Get(nodeId);

        var blockVerified = currentNode.VerifyBlock(@event.Block);

        _networkStorage.Update(currentNode);
        _networkBroker.Publish(blockVerified);
    }
}