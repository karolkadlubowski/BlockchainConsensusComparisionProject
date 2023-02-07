using BKiZA.ProofOfWork.Nodes.Events;
using BKiZA.Shared.Infrastructure.Metrics;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes;

namespace BKiZA.ProofOfWork.Nodes.Services;

public class MinerService
{
    private readonly INetworkStorage<Miner> _minerNetworkStorage;
    private readonly INetworkStorage<User> _userNetworkStorage;
    private readonly NetworkBroker _networkBroker;
    private readonly IMetricsCollector _metricsCollector;

    public MinerService(INetworkStorage<Miner> minerNetworkStorage,
        INetworkStorage<User> userNetworkStorage,
        NetworkBroker networkBroker,
        IMetricsCollector metricsCollector)
    {
        _minerNetworkStorage = minerNetworkStorage;
        _userNetworkStorage = userNetworkStorage;
        _networkBroker = networkBroker;
        _metricsCollector = metricsCollector;
    }

    public bool IsAlreadyDig { get; set; }

    public void AddMiner(string currentNodeId)
    {
        var currentMiner = _minerNetworkStorage.Get(currentNodeId);

        var miner = Miner.CreateNew(currentMiner);
        _minerNetworkStorage.Add(miner);
        _userNetworkStorage.Add(miner.UserAccount);

        _networkBroker.Publish(new MinerNodeAdded(miner));
    }

    public void AddUser(string currentNodeId)
    {
        var currentMiner = _minerNetworkStorage.Get(currentNodeId);

        var userNodeAdded = currentMiner.CreateUser();

        _userNetworkStorage.Add(userNodeAdded.User);
        _networkBroker.Publish(userNodeAdded);
    }

    public void DigBlock(string currentNodeId)
    {
        var currentMiner = _minerNetworkStorage.Get(currentNodeId);

        var (blockDigged, digTime) = currentMiner.DigBlock();
        _metricsCollector.CollectMetrics(m => m.AvgDigBlockTime = digTime);

        _minerNetworkStorage.Update(currentMiner);

        if (blockDigged is not null)
        {
            _networkBroker.Publish(blockDigged);
        }
    }
}