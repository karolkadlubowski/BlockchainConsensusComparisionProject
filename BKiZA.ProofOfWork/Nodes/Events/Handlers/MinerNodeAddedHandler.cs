using System.Linq;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes;

namespace BKiZA.ProofOfWork.Nodes.Events.Handlers;

public class MinerNodeAddedHandler : IEventHandler<MinerNodeAdded>
{
    private readonly INetworkStorage<Miner> _minerNetworkStorage;
    private readonly INetworkStorage<User> _userNetworkStorage;

    public MinerNodeAddedHandler(INetworkStorage<Miner> minerNetworkStorage,
        INetworkStorage<User> userNetworkStorage)
    {
        _minerNetworkStorage = minerNetworkStorage;
        _userNetworkStorage = userNetworkStorage;
    }

    public void Handle(string nodeId, MinerNodeAdded @event)
    {
        var miners = _minerNetworkStorage
            .Scan()
            .Where(m => m.NodeId != @event.Miner.NodeId);

        foreach (var miner in miners)
        {
            var currentNode = _minerNetworkStorage.Get(miner.NodeId);

            currentNode.AppendNode(@event.Miner);

            _minerNetworkStorage.Update(currentNode);
        }
    }
}