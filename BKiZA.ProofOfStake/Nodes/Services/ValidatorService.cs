using BKiZA.ProofOfStake.Nodes.Events;
using BKiZA.Shared.Infrastructure.Metrics;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes;

namespace BKiZA.ProofOfStake.Nodes.Services;

public class ValidatorService
{
    private readonly INetworkStorage<Validator> _validatorNetworkStorage;
    private readonly INetworkStorage<User> _userNetworkStorage;
    private readonly NetworkBroker _networkBroker;
    private readonly IMetricsCollector _metricsCollector;

    public ValidatorService(INetworkStorage<Validator> validatorNetworkStorage,
        INetworkStorage<User> userNetworkStorage,
        NetworkBroker networkBroker,
        IMetricsCollector metricsCollector
    )
    {
        _validatorNetworkStorage = validatorNetworkStorage;
        _userNetworkStorage = userNetworkStorage;
        _networkBroker = networkBroker;
        _metricsCollector = metricsCollector;
    }

    public void AddValidator(string currentNodeId, string currentUserId)
    {
        var currentUser = _userNetworkStorage.Get(currentUserId);
        var currentValidator = _validatorNetworkStorage.Get(currentNodeId);

        if (currentUser.Balance > 0)
        {
            var newValidator = Validator.TransformUserToValidator(currentValidator, currentUser);
            _validatorNetworkStorage.Add(newValidator);

            _networkBroker.Publish(new ValidatorNodeAdded(newValidator, currentUserId));
        }
    }

    public string AddUser(string currentNodeId)
    {
        var validator = _validatorNetworkStorage.Get(currentNodeId);

        var userNodeAdded = validator.CreateUser();

        _userNetworkStorage.Add(userNodeAdded.User);
        _networkBroker.Publish(userNodeAdded);
        return userNodeAdded.User.NodeId;
    }

    public void DigBlock(string currentNodeId)
    {
        var currentValidator = _validatorNetworkStorage.Get(currentNodeId);

        var (blockDigged, digTime) = currentValidator.DigBlock();

        _metricsCollector.CollectMetrics(m => m.AvgDigBlockTime = digTime);

        _validatorNetworkStorage.Update(currentValidator);

        if (blockDigged is not null)
        {
            _networkBroker.Publish(blockDigged);
        }
    }
}