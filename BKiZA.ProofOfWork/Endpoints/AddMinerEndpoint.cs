using System;
using BKiZA.ProofOfWork.Nodes.Services;
using BKiZA.Shared.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKiZA.ProofOfWork.Endpoints;

public class AddMinerEndpoint : ModuleEndpoint
{
    public override string Route => "miners/{creatorId}";
    public override HttpMethod Method => HttpMethod.Post;

    public override Delegate Handler { get; } = ([FromRoute] string creatorId,
        [FromServices] MinerService minerService) =>
    {
        minerService.AddMiner(creatorId);

        return Results.Ok();
    };
}