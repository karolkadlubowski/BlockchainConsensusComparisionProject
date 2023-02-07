using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes.Events;

namespace BKiZA.ProofOfWork.Nodes.Events.Handlers;

public class MinerTransactionSentHandler : IEventHandler<TransactionSent>
{
    private readonly INetworkStorage<Miner> _minerNetworkStorage;

    public MinerTransactionSentHandler(INetworkStorage<Miner> minerNetworkStorage)
    {
        _minerNetworkStorage = minerNetworkStorage;
    }

    public void Handle(string nodeId, TransactionSent @event)
    {
        foreach (var miner in _minerNetworkStorage.Scan())
        {
            var currentMiner = _minerNetworkStorage.Get(miner.NodeId);

            currentMiner.AppendTransaction(@event.Transaction);

            _minerNetworkStorage.Update(currentMiner);
        }
    }
}