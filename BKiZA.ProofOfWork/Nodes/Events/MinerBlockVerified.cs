using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Nodes;

namespace BKiZA.ProofOfWork.Nodes.Events;

public record MinerBlockVerified(Block Block, string NodeIdToVerify , bool Accepted) : IEvent;