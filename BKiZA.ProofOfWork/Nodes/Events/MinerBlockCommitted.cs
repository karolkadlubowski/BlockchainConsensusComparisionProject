using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Nodes;

namespace BKiZA.ProofOfWork.Nodes.Events;

public record MinerBlockCommitted(Block Block, string NodeId) : IEvent;