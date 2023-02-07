using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace BKiZA.Shared.Infrastructure;

public static class Registration
{
    private const string ApiPrefix = "api/";

    public static void MapModuleEndpoints(this IEndpointRouteBuilder builder, Assembly[] assemblies)
    {
        var endpointTypes = assemblies.SelectMany(a => a.GetTypes())
            .Where(type => type.IsAssignableTo(typeof(ModuleEndpoint))
                           && type.IsClass
                           && !type.IsAbstract)
            .ToArray();

        foreach (var endpointType in endpointTypes)
        {
            var endpoint = Activator.CreateInstance(endpointType) as ModuleEndpoint;

            if (endpoint is null)
            {
                continue;
            }

            builder.MapEndpoint(endpoint);
        }
    }

    private static void MapEndpoint(this IEndpointRouteBuilder builder,
        ModuleEndpoint endpoint,
        string groupRoutePrefix = "")
    {
        var handlerBuilder = endpoint.Method switch
        {
            ModuleEndpoint.HttpMethod.Get => builder.MapGet($"{ApiPrefix}{groupRoutePrefix}{endpoint.Route}", endpoint.Handler),
            ModuleEndpoint.HttpMethod.Post => builder.MapPost($"{ApiPrefix}{groupRoutePrefix}{endpoint.Route}", endpoint.Handler),
            ModuleEndpoint.HttpMethod.Put => builder.MapPut($"{ApiPrefix}{groupRoutePrefix}{endpoint.Route}", endpoint.Handler),
            ModuleEndpoint.HttpMethod.Delete => builder.MapDelete($"{ApiPrefix}{groupRoutePrefix}{endpoint.Route}", endpoint.Handler),
            _ => throw new InvalidOperationException($"Unsupported HTTP method: {endpoint.Method}")
        };

        if (endpoint.RequireAuthentication)
        {
            handlerBuilder.RequireAuthorization(endpoint.AuthorizationPolicies);
        }

        endpoint.Configure(handlerBuilder);
    }
}