using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;

namespace BKiZA.ProofOfStake.Nodes.Events.Handlers;

public class ValidatorNodeAddedHandler : IEventHandler<ValidatorNodeAdded>
{
    private readonly INetworkStorage<Validator> _networkStorage;

    public ValidatorNodeAddedHandler(INetworkStorage<Validator> networkStorage)
    {
        _networkStorage = networkStorage;
    }
    public void Handle(string nodeId, ValidatorNodeAdded @event)
    {
        if(nodeId != @event.userNodeId)
        {
            var validator = _networkStorage.Get(nodeId);

            validator.ClearUserBalance(@event.userNodeId);
            validator.AppendNode(@event.NewValidator);

            //_networkStorage.Update(validator);
        }
    }
}