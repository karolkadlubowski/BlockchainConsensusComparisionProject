using System;

namespace BKiZA.Shared.Infrastructure.Metrics;

public class MetricsCollector : IMetricsCollector
{
    private readonly Metrics _metrics = new Metrics();

    public Metrics GetMetrics()
        => _metrics.Clone();

    public void CollectMetrics(Action<Metrics> collect)
    {
        collect(_metrics);
        _metrics.CreatedAt = DateTime.Now;
    }
}