using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Nodes;

namespace BKiZA.ProofOfStake.Nodes.Events;

public record ValidatorBlockCommitted(Block Block, string ValidatorId) : IEvent;