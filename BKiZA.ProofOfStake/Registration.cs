using BKiZA.ProofOfStake.Infrastructure.Metrics;
using BKiZA.ProofOfStake.Network;
using BKiZA.ProofOfStake.Nodes;
using BKiZA.ProofOfStake.Nodes.Services;
using BKiZA.Shared.Network;
using Microsoft.Extensions.DependencyInjection;

namespace BKiZA.ProofOfStake;

public static class Registration
{
    public static IServiceCollection AddProofOfStake(this IServiceCollection services)
    {
        services.AddSingleton<INetworkStorage<Validator>, ValidatorNetworkStorage>();

        services.AddSingleton<ValidatorService>();

        services.AddMetrics();

        services.AddHostedService<ProofOfStakeNetworkDispatcherJob>();

        return services;
    }
}