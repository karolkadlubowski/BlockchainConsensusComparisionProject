using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;

namespace BKiZA.ProofOfWork.Nodes.Events.Handlers;

public class MinerBlockDiggedHandler : IEventHandler<MinerBlockDigged>
{
    private readonly INetworkStorage<Miner> _networkStorage;
    private readonly NetworkBroker _networkBroker;

    public MinerBlockDiggedHandler(INetworkStorage<Miner> networkStorage,
        NetworkBroker networkBroker)
    {
        _networkStorage = networkStorage;
        _networkBroker = networkBroker;
    }

    public void Handle(string nodeId, MinerBlockDigged @event)
    {
        foreach (var miner in _networkStorage.Scan())
        {
            var currentNode = _networkStorage.Get(miner.NodeId);

            var blockVerified = currentNode.VerifyBlock(@event.Block, @event.MinerNodeId);

            _networkStorage.Update(currentNode);
            _networkBroker.Publish(blockVerified);
        }
    }
}