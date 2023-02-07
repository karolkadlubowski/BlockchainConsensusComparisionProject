using BKiZA.Shared.Infrastructure;

namespace BKiZA.ProofOfStake.Nodes.Events;

public record ValidatorNodeAdded(Validator NewValidator, string userNodeId) : IEvent;