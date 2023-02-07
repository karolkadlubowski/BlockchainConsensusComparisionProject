using System;

namespace BKiZA.Shared.Infrastructure.Metrics;

public sealed class Metrics
{
    public int NodesCount { get; set; }
    public int PendingTransactionsCount { get; set; }
    public long AvgDigBlockTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Metrics Clone()
        => new Metrics
        {
            NodesCount = NodesCount,
            PendingTransactionsCount = PendingTransactionsCount,
            AvgDigBlockTime = AvgDigBlockTime,
            CreatedAt = CreatedAt
        };
}