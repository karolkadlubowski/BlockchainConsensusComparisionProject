using System;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKiZA.Shared.Endpoints;

public class GetUsersEndpoint : ModuleEndpoint
{
    public override string Route => "users";
    public override HttpMethod Method => HttpMethod.Get;

    public override Delegate Handler { get; } = ([FromServices] INetworkStorage<User> networkStorage) =>
    {
        var users = networkStorage.Scan();

        return Results.Ok(users);
    };
}