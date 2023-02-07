using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BKiZA.ProofOfWork.Nodes;
using BKiZA.Shared.Infrastructure.Metrics;
using BKiZA.Shared.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BKiZA.ProofOfWork.Infrastructure.Metrics;

public class ProofOfWorkMetricsWorker : BackgroundService
{
    private readonly ILogger<ProofOfWorkMetricsWorker> _logger;
    private readonly IMetricsCollector _metricsCollector;
    private readonly IMetricsHistory _metricsHistory;
    private readonly INetworkStorage<Miner> _networkStorage;

    public ProofOfWorkMetricsWorker(ILogger<ProofOfWorkMetricsWorker> logger,
        IMetricsCollector metricsCollector,
        IMetricsHistory metricsHistory,
        INetworkStorage<Miner> networkStorage)
    {
        _logger = logger;
        _metricsCollector = metricsCollector;
        _metricsHistory = metricsHistory;
        _networkStorage = networkStorage;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Collecting metrics...");

            _metricsCollector.CollectMetrics(m => m.NodesCount = _networkStorage.NodesCount);
            _metricsCollector.CollectMetrics(m => m.PendingTransactionsCount = _networkStorage.Scan()
                .SelectMany(miner => miner.CurrentTransactions)
                .DistinctBy(trx => trx.TransactionId)
                .Count());

            _metricsHistory.Save();

            _logger.LogInformation("Metrics collected successfully at: {Now}", DateTime.Now);

            _logger.LogInformation("Metrics history:\r\n{Metrics}", JsonSerializer.Serialize(_metricsHistory.GetHistory(), new JsonSerializerOptions
            {
                WriteIndented = true
            }));

            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }
}