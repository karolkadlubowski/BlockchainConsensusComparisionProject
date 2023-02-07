using BKiZA.Shared.Infrastructure;

namespace BKiZA.Shared.Nodes.Events;

public record TransactionSent(Transaction Transaction) : IEvent;