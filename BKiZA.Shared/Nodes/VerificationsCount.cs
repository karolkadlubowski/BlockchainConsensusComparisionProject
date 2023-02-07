namespace BKiZA.Shared.Nodes;

public record VerificationsCount
{
    public VerificationsCount(int nodesCount)
    {
        NeededVerificationsCount = nodesCount / 2;
    }

    public int NeededVerificationsCount { get; }

    public static implicit operator int(VerificationsCount instance)
        => instance.NeededVerificationsCount;
}