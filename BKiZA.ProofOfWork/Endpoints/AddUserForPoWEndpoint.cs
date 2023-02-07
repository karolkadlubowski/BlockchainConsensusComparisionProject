using System;
using BKiZA.ProofOfWork.Nodes.Services;
using BKiZA.Shared.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKiZA.ProofOfWork.Endpoints;

public class AddUserForPoWEndpoint : ModuleEndpoint
{
    public override string Route => "users/pow/{minerId}";
    public override HttpMethod Method => HttpMethod.Post;

    public override Delegate Handler { get; } = ([FromRoute] string minerId,
        [FromServices] MinerService minerService) =>
    {
        minerService.AddUser(minerId);

        return Results.Ok();
    };
}