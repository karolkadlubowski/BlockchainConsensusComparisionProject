using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes.Events;

namespace BKiZA.ProofOfStake.Nodes.Events.Handlers;

public class ValidatorUserNodeAddedHandler : IEventHandler<UserNodeAdded>
{
    private readonly INetworkStorage<Validator> _networkStorage;

    public ValidatorUserNodeAddedHandler(INetworkStorage<Validator> networkStorage)
    {
        _networkStorage = networkStorage;
    }

    public void Handle(string nodeId, UserNodeAdded @event)
    {
        var currentNode = _networkStorage.Get(nodeId);

        currentNode.AppendNode(@event.User);

        _networkStorage.Update(currentNode);
    }
}