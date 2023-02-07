using System.Reflection;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Infrastructure.Metrics;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes;
using BKiZA.Shared.Nodes.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BKiZA.Shared;

public static class Registration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddSingleton<EventDispatcher>();

        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        services.AddSingleton<IMetricsCollector, MetricsCollector>();
        services.AddSingleton<IMetricsHistory, MetricsHistory>();

        return services;
    }

    public static IServiceCollection AddNetwork(this IServiceCollection services)
    {
        services.AddSingleton<INetworkStorage<User>, UserNetworkStorage>();

        services.AddSingleton<NetworkBroker>();
        services.AddSingleton<NetworkEventChannel>();

        return services;
    }

    public static IServiceCollection AddNodes(this IServiceCollection services)
    {
        services.AddSingleton<UserService>();

        return services;
    }
}