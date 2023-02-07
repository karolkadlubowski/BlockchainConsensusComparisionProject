using BKiZA.Shared.Infrastructure;

namespace BKiZA.ProofOfStake.Nodes.Events;

public record ValidatorChosen(string NextValidatorId) : IEvent;