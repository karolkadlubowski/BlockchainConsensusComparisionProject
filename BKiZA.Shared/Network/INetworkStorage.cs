using System.Collections.Generic;

namespace BKiZA.Shared.Network;

public interface INetworkStorage<TNode> where TNode : class
{
    int NodesCount { get; }

    TNode Get(string nodeId);
    List<TNode> Scan();
    void Add(TNode node);
    void Update(TNode node);
}