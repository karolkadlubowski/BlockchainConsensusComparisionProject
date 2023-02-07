using System.Collections.Generic;
using System.Linq;
using BKiZA.Shared.Network.Exceptions;
using BKiZA.Shared.Nodes;

namespace BKiZA.Shared.Network;

public class UserNetworkStorage : INetworkStorage<User>
{
    private readonly List<User> _users = new List<User>(Enumerable
            .Range(1, 3)
            .Select(index => new User($"{index}", 1))
            .Concat(new[] { User.Default }))
        .ToList();

    public int NodesCount => _users.Count;

    public User Get(string nodeId)
    {
        var userFromNetwork = _users.FirstOrDefault(m => m.NodeId == nodeId)
                              ?? throw new NodeNotFoundException(nodeId);

        return (User)userFromNetwork.Clone();
    }

    public List<User> Scan()
        => _users
            .Select(u => u.Clone())
            .Cast<User>()
            .ToList();

    public void Add(User node)
    {
        if (_users.All(m => m.NodeId != node.NodeId))
        {
            _users.Add(node);
        }
    }

    public void Update(User node)
    {
        var nodeIndex = _users.FindIndex(n => n.NodeId == node.NodeId);
        _users[nodeIndex] = node;
    }
}