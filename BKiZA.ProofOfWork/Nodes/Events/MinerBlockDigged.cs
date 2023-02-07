using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Nodes;

namespace BKiZA.ProofOfWork.Nodes.Events;

public record MinerBlockDigged(Block Block, string MinerNodeId) : IEvent;