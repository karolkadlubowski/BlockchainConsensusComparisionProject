using System.Collections.Generic;
using System.Linq;

namespace BKiZA.Shared.Infrastructure.Metrics;

public class MetricsHistory : IMetricsHistory
{
    private readonly ICollection<Metrics> _metricsHistory = new List<Metrics>();

    private readonly IMetricsCollector _metricsCollector;

    public MetricsHistory(IMetricsCollector metricsCollector)
    {
        _metricsCollector = metricsCollector;
    }

    public IReadOnlyList<Metrics> GetHistory()
        => _metricsHistory.ToList();

    public void Save()
        => _metricsHistory.Add(_metricsCollector.GetMetrics());
}