using BKiZA.Shared.Infrastructure;

namespace BKiZA.Shared.Nodes.Events;

public record UserNodeAdded(User User) : IEvent;