using System;
using System.Linq;
using System.Threading.Tasks;
using BKiZA.ProofOfWork.Nodes.Services;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Infrastructure.Metrics;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes;
using BKiZA.Shared.Nodes.Events;
using Microsoft.Extensions.Logging;

namespace BKiZA.ProofOfWork.Nodes.Events.Handlers;

public class MinerBlockCommittedHandler : IEventHandler<MinerBlockCommitted>
{
    private readonly INetworkStorage<Miner> _minerNetworkStorage;
    private readonly INetworkStorage<User> _userNetworkStorage;
    private readonly NetworkBroker _networkBroker;
    private readonly MinerService _minerService;
    private readonly ILogger<MinerBlockCommittedHandler> _logger;
    private readonly IMetricsCollector _metricsCollector;

    public MinerBlockCommittedHandler(INetworkStorage<Miner> minerNetworkStorage,
        INetworkStorage<User> userNetworkStorage,
        NetworkBroker networkBroker,
        MinerService minerService,
        ILogger<MinerBlockCommittedHandler> logger,
        IMetricsCollector metricsCollector)
    {
        _minerNetworkStorage = minerNetworkStorage;
        _userNetworkStorage = userNetworkStorage;
        _networkBroker = networkBroker;
        _minerService = minerService;
        _logger = logger;
        _metricsCollector = metricsCollector;
    }

    public void Handle(string nodeId, MinerBlockCommitted @event)
    {
        foreach (var miner in _minerNetworkStorage.Scan())
        {
            var currentMiner = _minerNetworkStorage.Get(miner.NodeId);

            currentMiner.AppendBlock(@event.Block, @event.NodeId);

            _minerNetworkStorage.Update(currentMiner);

            foreach (var user in currentMiner.CurrentUsers)
            {
                _userNetworkStorage.Update(user);
            }
        }

        var randomNumber = new Random();
        var miners = _minerNetworkStorage.Scan()
            .OrderBy(m => randomNumber.Next())
            .ToList();

        Parallel.ForEach(miners, miner =>
        {
            var currentMiner = _minerNetworkStorage.Get(miner.NodeId);

            var (blockDigged, digTime) = currentMiner.DigBlock();
            _metricsCollector.CollectMetrics(m => m.AvgDigBlockTime = digTime);

            _minerNetworkStorage.Update(currentMiner);

            Task.Delay(TimeSpan.FromMilliseconds(50));
            
            if (blockDigged is not null && _minerService.IsAlreadyDig)
            {
                _minerService.IsAlreadyDig = false;
                _logger.LogInformation("Block with hash: '{Hash}'," +
                                       " previous hash: '{PreviousHash}'," +
                                       " transactions: {TransactionsCount} successfully added to chain" +
                                       "by miner with Id: {MinerId}",
                    @event.Block.Hash,
                    @event.Block.PreviousHash,
                    @event.Block.Transactions.Count,
                    miner.NodeId);
                _networkBroker.Publish(blockDigged);
            }
        });
    }
}