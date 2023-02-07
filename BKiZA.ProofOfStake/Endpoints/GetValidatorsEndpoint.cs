using System;
using BKiZA.ProofOfStake.Nodes;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKiZA.ProofOfStake.Endpoints;

public class GetValidatorsEndpoint : ModuleEndpoint
{
    public override string Route => "validators";
    public override HttpMethod Method => HttpMethod.Get;

    public override Delegate Handler { get; } = ([FromServices] INetworkStorage<Validator> networkStorage) =>
    {
        var users = networkStorage.Scan();

        return Results.Ok(users);
    };
}