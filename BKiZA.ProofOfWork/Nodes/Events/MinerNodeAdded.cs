using BKiZA.Shared.Infrastructure;

namespace BKiZA.ProofOfWork.Nodes.Events;

public record MinerNodeAdded(Miner Miner) : IEvent;