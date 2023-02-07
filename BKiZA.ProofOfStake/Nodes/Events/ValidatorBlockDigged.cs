using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Nodes;

namespace BKiZA.ProofOfStake.Nodes.Events;

public record ValidatorBlockDigged(Block Block) : IEvent;