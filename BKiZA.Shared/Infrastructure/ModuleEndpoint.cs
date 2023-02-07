using System;
using Microsoft.AspNetCore.Builder;

namespace BKiZA.Shared.Infrastructure;

public abstract class ModuleEndpoint
{
    public abstract string Route { get; }
    public abstract HttpMethod Method { get; }
    public abstract Delegate Handler { get; }
    public virtual bool RequireAuthentication { get; } = false;
    public virtual string[] AuthorizationPolicies { get; } = Array.Empty<string>();

    public virtual void Configure(RouteHandlerBuilder builder)
    {
    }

    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete
    }
}