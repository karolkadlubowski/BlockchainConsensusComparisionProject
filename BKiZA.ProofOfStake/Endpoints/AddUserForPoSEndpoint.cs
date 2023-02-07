using System;
using BKiZA.ProofOfStake.Nodes.Services;
using BKiZA.Shared.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKiZA.ProofOfStake.Endpoints;

public class AddUserForPoSEndpoint : ModuleEndpoint
{
    public override string Route => "users/pos/{validatorId}";
    public override HttpMethod Method => HttpMethod.Post;

    public override Delegate Handler { get; } = (
        [FromRoute] string validatorId,
        [FromServices] ValidatorService validatorService) =>
    {
        var userId = validatorService.AddUser(validatorId);

        return Results.Ok(userId);
    };
}