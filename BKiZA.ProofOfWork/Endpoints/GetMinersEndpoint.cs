using System;
using BKiZA.ProofOfWork.Nodes;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKiZA.ProofOfWork.Endpoints;

public class GetMinersEndpoint : ModuleEndpoint
{
    public override string Route => "miners";
    public override HttpMethod Method => HttpMethod.Get;

    public override Delegate Handler { get; } = ([FromServices] INetworkStorage<Miner> networkStorage) =>
    {
        var users = networkStorage.Scan();

        return Results.Ok(users);
    };
}