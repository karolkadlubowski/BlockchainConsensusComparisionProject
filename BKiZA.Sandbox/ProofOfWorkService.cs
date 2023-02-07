using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BKiZA.ProofOfWork.Nodes;
using BKiZA.ProofOfWork.Nodes.Services;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes;
using BKiZA.Shared.Nodes.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BKiZA.Sandbox;

public class ProofOfWorkService : BackgroundService
{
    private readonly MinerService _minerService;
    private readonly UserService _userService;
    private readonly INetworkStorage<Miner> _minerNetworkStorage;
    private readonly INetworkStorage<User> _userNetworkStorage;
    private readonly ILogger<ProofOfWorkService> _logger;

    public ProofOfWorkService(MinerService minerService,
        UserService userService,
        INetworkStorage<Miner> minerNetworkStorage,
        INetworkStorage<User> userNetworkStorage,
        ILogger<ProofOfWorkService> logger)
    {
        _minerService = minerService;
        _userService = userService;
        _minerNetworkStorage = minerNetworkStorage;
        _userNetworkStorage = userNetworkStorage;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        const string RootId = "A";

        await Task.Delay(TimeSpan.FromSeconds(2));

        _minerService.AddMiner(RootId);

        await Task.Delay(TimeSpan.FromSeconds(2));

        _userService.SendTransaction("1", "2");

        await Task.Delay(TimeSpan.FromSeconds(2));

        _minerService.DigBlock(RootId);

        await Task.CompletedTask;
    }

    private void LogCurrentNetworkState()
    {
        var logBuilder = new StringBuilder("Current network state:\r\n")
            .AppendLine(string.Join(string.Empty, Enumerable.Repeat("-", 40)));

        _minerNetworkStorage.Scan()
            .ForEach(n => logBuilder = logBuilder
                .AppendLine(
                    $"Node ID: {n.NodeId} | Block chain: [{string.Join(" ---> ", n.BlockChain.Chain.Select(b => b.Hash))}] | \r\n" +
                    $"Users: {string.Join(" | ", n.CurrentUsers.Select(cn => $"( ID: {cn.NodeId} | Balance: {cn.Balance} )"))}\r\n")
                .Append(string.Join(string.Empty, Enumerable.Repeat("-", 80)))
                .AppendLine());

        _logger.LogInformation(logBuilder.ToString());
    }

    private void LogUsersState()
    {
        var logBuilder = new StringBuilder("Current users state:\r\n")
            .AppendLine(string.Join(string.Empty, Enumerable.Repeat("-", 40)));

        _userNetworkStorage.Scan()
            .ForEach(u => logBuilder = logBuilder
                .AppendLine($"User ID: {u.NodeId} | Balance: {u.Balance}"));

        logBuilder.AppendLine(string.Join(string.Empty, Enumerable.Repeat("-", 80)));

        _minerNetworkStorage.Scan()
            .ForEach(m => logBuilder = logBuilder
                .AppendLine($"Node ID: {m.NodeId}\r\n" +
                            $"Transactions: [{string.Join(string.Empty, m.CurrentTransactions.Select(t => $"( Trx ID: {t.TransactionId} | From: {t.From} | To: {t.To} | Value: {t.Value} )"))}]")
                .AppendLine(string.Join(string.Empty, Enumerable.Repeat("-", 80))));

        _logger.LogInformation(logBuilder.ToString());
    }
}