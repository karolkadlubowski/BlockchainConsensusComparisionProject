using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes.Events;

namespace BKiZA.ProofOfStake.Nodes.Events.Handlers;

public class ValidatorTransactionSentHandler : IEventHandler<TransactionSent>
{
    private readonly INetworkStorage<Validator> _validatorNetworkStorage;

    public ValidatorTransactionSentHandler(INetworkStorage<Validator> minerNetworkStorage)
    {
        _validatorNetworkStorage = minerNetworkStorage;
    }

    public void Handle(string nodeId, TransactionSent @event)
    {
        var currentValidator = _validatorNetworkStorage.Get(nodeId);

        currentValidator.AppendTransaction(@event.Transaction);

        _validatorNetworkStorage.Update(currentValidator);
    }
}