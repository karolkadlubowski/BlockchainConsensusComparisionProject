using System.Collections.Generic;

namespace BKiZA.Shared.Infrastructure.Metrics;

public interface IMetricsHistory
{
    IReadOnlyList<Metrics> GetHistory();
    void Save();
}