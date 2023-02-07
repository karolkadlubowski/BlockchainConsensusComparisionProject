using System.Collections.Generic;
using System.Linq;
using BKiZA.ProofOfWork.Nodes;
using BKiZA.Shared.Network;
using BKiZA.Shared.Network.Exceptions;

namespace BKiZA.ProofOfWork.Network;

public class MinerNetworkStorage : INetworkStorage<Miner>
{
    private readonly List<Miner> _miners = new List<Miner>
    {
        Miner.CreateRoot("A")
    };

    public int NodesCount => _miners.Count;

    public Miner Get(string nodeId)
    {
        var minerFromNetwork = _miners.FirstOrDefault(m => m.NodeId == nodeId)
                               ?? throw new NodeNotFoundException(nodeId);

        return (Miner)minerFromNetwork.Clone();
    }

    public List<Miner> Scan()
        => _miners
            .Select(m => m.Clone())
            .Cast<Miner>()
            .ToList();

    public void Add(Miner node)
    {
        if (_miners.All(m => m.NodeId != node.NodeId))
        {
            _miners.Add(node);
        }
    }

    public void Update(Miner node)
    {
        var nodeIndex = _miners.FindIndex(n => n.NodeId == node.NodeId);
        _miners[nodeIndex] = node;
    }
}