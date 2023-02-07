using System;
using BKiZA.ProofOfStake.Nodes.Services;
using BKiZA.Shared.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKiZA.ProofOfStake.Endpoints;

public class AddValidatorEndpoint : ModuleEndpoint
{
    public override string Route => "validators/{creatorId}/users/{userId}";
    public override HttpMethod Method => HttpMethod.Post;

    public override Delegate Handler { get; } = ([FromRoute] string creatorId,
        [FromRoute] string userId,
        [FromServices] ValidatorService validatorService) =>
    {
        validatorService.AddValidator(creatorId, userId);

        return Results.Ok();
    };
}