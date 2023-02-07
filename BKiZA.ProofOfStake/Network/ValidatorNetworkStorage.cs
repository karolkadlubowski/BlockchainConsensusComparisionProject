using System.Collections.Generic;
using System.Linq;
using BKiZA.ProofOfStake.Nodes;
using BKiZA.Shared.Network;
using BKiZA.Shared.Network.Exceptions;
using BKiZA.Shared.Nodes;

namespace BKiZA.ProofOfStake.Network;

public class ValidatorNetworkStorage: INetworkStorage<Validator>
{
    private readonly List<Validator> _validators = new List<Validator>
    {
        Validator.CreateRoot(User.Default)
    };

    public int NodesCount => _validators.Count;

    public Validator Get(string nodeId)
    {
        var validatorFromNetwork = _validators.FirstOrDefault(m => m.NodeId == nodeId)
                               ?? throw new NodeNotFoundException(nodeId);

        return (Validator)validatorFromNetwork.Clone();
    }

    public List<Validator> Scan()
        => _validators
            .Select(m => m.Clone())
            .Cast<Validator>()
            .ToList();

    public void Add(Validator node)
    {
        if (_validators.All(m => m.NodeId != node.NodeId))
        {
            _validators.Add(node);
        }
    }

    public void Update(Validator node)
    {
        var nodeIndex = _validators.FindIndex(n => n.NodeId == node.NodeId);
        _validators[nodeIndex] = node;
    }  
}