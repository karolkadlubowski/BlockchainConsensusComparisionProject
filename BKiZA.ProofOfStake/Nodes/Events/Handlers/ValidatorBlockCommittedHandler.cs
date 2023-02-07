using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes;
using Microsoft.Extensions.Logging;

namespace BKiZA.ProofOfStake.Nodes.Events.Handlers;

public class ValidatorBlockCommittedHandler : IEventHandler<ValidatorBlockCommitted>
{
    private readonly INetworkStorage<Validator> _validatorNetworkStorage;
    private readonly INetworkStorage<User> _userNetworkStorage;
    private readonly ILogger<ValidatorBlockCommittedHandler> _logger;

    public ValidatorBlockCommittedHandler(INetworkStorage<Validator> validatorNetworkStorage,
        INetworkStorage<User> userNetworkStorage,
        ILogger<ValidatorBlockCommittedHandler> logger)
    {
        _validatorNetworkStorage = validatorNetworkStorage;
        _userNetworkStorage = userNetworkStorage;
        _logger = logger;
    }

    public void Handle(string nodeId, ValidatorBlockCommitted @event)
    {
        var currentValidator = _validatorNetworkStorage.Get(nodeId);

        currentValidator.AppendBlock(@event.Block, @event.ValidatorId);

        _validatorNetworkStorage.Update(currentValidator);
        foreach (var user in currentValidator.CurrentUsers)
        {
            _userNetworkStorage.Update(user);
        }

        if (nodeId == @event.ValidatorId)
        {
            _logger.LogInformation("Validator '{validatorId}' forged block with hash: '{Hash}', previous hash: '{PreviousHash}', transactions: {TransactionsCount} successfully added to chain\n" +
                                   "Validator's account: '{validatorAccount}'",
                @event.ValidatorId,
                @event.Block.Hash,
                @event.Block.PreviousHash,
                @event.Block.Transactions.Count,
                currentValidator.UserAccount.Balance.ToString());
        }
    }
}