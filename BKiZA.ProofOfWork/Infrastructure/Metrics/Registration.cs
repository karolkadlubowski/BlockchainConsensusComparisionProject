using Microsoft.Extensions.DependencyInjection;

namespace BKiZA.ProofOfWork.Infrastructure.Metrics;

public static class Registration
{
    public static IServiceCollection AddMetrics(this IServiceCollection services)
    {
        services.AddHostedService<ProofOfWorkMetricsWorker>();

        return services;
    }
}