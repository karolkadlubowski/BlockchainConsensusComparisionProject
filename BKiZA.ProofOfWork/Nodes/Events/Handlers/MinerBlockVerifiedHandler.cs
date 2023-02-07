using BKiZA.ProofOfWork.Nodes.Services;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;

namespace BKiZA.ProofOfWork.Nodes.Events.Handlers;

public class MinerBlockVerifiedHandler : IEventHandler<MinerBlockVerified>
{
    private readonly INetworkStorage<Miner> _networkStorage;
    private readonly NetworkBroker _networkBroker;
    private readonly MinerService _minerService;

    public MinerBlockVerifiedHandler(INetworkStorage<Miner> networkStorage,
        NetworkBroker networkBroker,
        MinerService minerService)
    {
        _networkStorage = networkStorage;
        _networkBroker = networkBroker;
        _minerService = minerService;
    }

    public void Handle(string nodeId, MinerBlockVerified @event)
    {
        var currentMiner = _networkStorage.Get(@event.NodeIdToVerify);

        var blockCommitted = currentMiner.TryToCommitBlock(@event, _networkStorage.NodesCount);

        _networkStorage.Update(currentMiner);

        if (blockCommitted is not null && _minerService.IsAlreadyDig is false)
        {
            _networkBroker.Publish(blockCommitted);
            _minerService.IsAlreadyDig = true;
        }
    }
}