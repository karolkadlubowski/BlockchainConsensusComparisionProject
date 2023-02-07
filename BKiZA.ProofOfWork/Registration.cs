using BKiZA.ProofOfWork.Infrastructure.Metrics;
using BKiZA.ProofOfWork.Network;
using BKiZA.ProofOfWork.Nodes;
using BKiZA.ProofOfWork.Nodes.Services;
using BKiZA.Shared.Network;
using Microsoft.Extensions.DependencyInjection;

namespace BKiZA.ProofOfWork;

public static class Registration
{
    public static IServiceCollection AddProofOfWork(this IServiceCollection services)
    {
        services.AddSingleton<INetworkStorage<Miner>, MinerNetworkStorage>();

        services.AddSingleton<MinerService>();

        services.AddMetrics();
        
        services.AddHostedService<ProofOfWorkNetworkDispatcherJob>();


        return services;
    }
}