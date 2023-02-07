using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BKiZA.Shared.Infrastructure;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapEndpoint(this IEndpointRouteBuilder endpointRouteBuilder, ModuleEndpoint.HttpMethod method, string route, Delegate handler,
        bool requireAuthentication = false,
        IEnumerable<string> roles = null,
        string name = null,
        string displayName = null,
        string[] tags = null)
    {
        var routeBuilder = method switch
        {
            ModuleEndpoint.HttpMethod.Get => endpointRouteBuilder.MapGet(route, handler),
            ModuleEndpoint.HttpMethod.Post => endpointRouteBuilder.MapPost(route, handler),
            ModuleEndpoint.HttpMethod.Put => endpointRouteBuilder.MapPut(route, handler),
            ModuleEndpoint.HttpMethod.Delete => endpointRouteBuilder.MapDelete(route, handler),
            _ => throw new InvalidOperationException($"Unsupported HTTP method: {method}")
        };

        if (requireAuthentication)
        {
            routeBuilder.RequireAuthorization(roles?.ToArray() ?? Array.Empty<string>());
        }

        if (string.IsNullOrWhiteSpace(name) is false)
        {
            routeBuilder.WithName(name);
        }

        if (string.IsNullOrWhiteSpace(displayName) is false)
        {
            routeBuilder.WithDisplayName(displayName);
        }

        if (tags?.Any() ?? false)
        {
            routeBuilder.WithTags(tags);
        }

        return endpointRouteBuilder;
    }

    public static IEndpointRouteBuilder Get(this IEndpointRouteBuilder endpointRouteBuilder, string route, Delegate handler,
        bool requireAuthentication = false,
        IEnumerable<string> roles = null,
        string name = null,
        string displayName = null,
        string[] tags = null)
        => endpointRouteBuilder.MapEndpoint(ModuleEndpoint.HttpMethod.Get,
            route, handler,
            requireAuthentication: requireAuthentication,
            roles: roles,
            name: name,
            displayName: displayName,
            tags: tags);

    public static IEndpointRouteBuilder Post(this IEndpointRouteBuilder endpointRouteBuilder, string route, Delegate handler,
        bool requireAuthentication = false,
        IEnumerable<string> roles = null,
        string name = null,
        string displayName = null,
        string[] tags = null)
        => endpointRouteBuilder.MapEndpoint(ModuleEndpoint.HttpMethod.Post,
            route, handler,
            requireAuthentication: requireAuthentication,
            roles: roles,
            name: name,
            displayName: displayName,
            tags: tags);

    public static IEndpointRouteBuilder Put(this IEndpointRouteBuilder endpointRouteBuilder, string route, Delegate handler,
        bool requireAuthentication = false,
        IEnumerable<string> roles = null,
        string name = null,
        string displayName = null,
        string[] tags = null)
        => endpointRouteBuilder.MapEndpoint(ModuleEndpoint.HttpMethod.Put,
            route, handler,
            requireAuthentication: requireAuthentication,
            roles: roles,
            name: name,
            displayName: displayName,
            tags: tags);

    public static IEndpointRouteBuilder Delete(this IEndpointRouteBuilder endpointRouteBuilder, string route, Delegate handler,
        bool requireAuthentication = false,
        IEnumerable<string> roles = null,
        string name = null,
        string displayName = null,
        string[] tags = null)
        => endpointRouteBuilder.MapEndpoint(ModuleEndpoint.HttpMethod.Delete,
            route, handler,
            requireAuthentication: requireAuthentication,
            roles: roles,
            name: name,
            displayName: displayName,
            tags: tags);
}