using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;

namespace BKiZA.ProofOfStake.Nodes.Events.Handlers;

public class ValidatorBlockVerifiedHandler : IEventHandler<ValidatorBlockVerified>
{
    private readonly INetworkStorage<Validator> _networkStorage;
    private readonly NetworkBroker _networkBroker;

    public ValidatorBlockVerifiedHandler(INetworkStorage<Validator> networkStorage,
        NetworkBroker networkBroker)
    {
        _networkStorage = networkStorage;
        _networkBroker = networkBroker;
    }
    public void Handle(string nodeId, ValidatorBlockVerified @event)
    {
        var currentValidator = _networkStorage.Get(nodeId);

        var blockCommitted = currentValidator.TryToCommitBlock(@event, _networkStorage.NodesCount);

        _networkStorage.Update(currentValidator);
        
        if (blockCommitted is not null)
        {
            _networkBroker.Publish(blockCommitted);
            _networkBroker.Publish(new ValidatorChosen(currentValidator.ChooseNextValidator()));
        }
    }
}