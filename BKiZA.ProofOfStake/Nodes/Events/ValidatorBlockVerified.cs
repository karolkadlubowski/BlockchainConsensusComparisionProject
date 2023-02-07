using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Nodes;

namespace BKiZA.ProofOfStake.Nodes.Events;

public record ValidatorBlockVerified(Block Block, bool Accepted) : IEvent;