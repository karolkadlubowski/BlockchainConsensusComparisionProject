using Microsoft.Extensions.DependencyInjection;

namespace BKiZA.ProofOfStake.Infrastructure.Metrics;

public static class Registration
{
    public static IServiceCollection AddMetrics(this IServiceCollection services)
    {
        services.AddHostedService<ProofOfStakeMetricsWorker>();

        return services;
    }
}