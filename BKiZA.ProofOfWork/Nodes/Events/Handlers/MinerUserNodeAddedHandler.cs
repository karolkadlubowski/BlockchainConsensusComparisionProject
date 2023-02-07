using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes.Events;

namespace BKiZA.ProofOfWork.Nodes.Events.Handlers;

public class MinerUserNodeAddedHandler : IEventHandler<UserNodeAdded>
{
    private readonly INetworkStorage<Miner> _networkStorage;

    public MinerUserNodeAddedHandler(INetworkStorage<Miner> networkStorage)
    {
        _networkStorage = networkStorage;
    }

    public void Handle(string nodeId, UserNodeAdded @event)
    {
        foreach (var miner in _networkStorage.Scan())
        {
            var currentNode = _networkStorage.Get(miner.NodeId);

            currentNode.AppendNode(@event.User);

            _networkStorage.Update(currentNode);
        }
    }
}