using System;

namespace BKiZA.Shared.Infrastructure.Metrics;

public interface IMetricsCollector
{
    Metrics GetMetrics();
    void CollectMetrics(Action<Metrics> collect);
}