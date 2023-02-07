using System;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Nodes.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKiZA.Shared.Endpoints;

public class SendTransactionEndpoint : ModuleEndpoint
{
    public override string Route => "transactions/{fromId}/to/{toId}";
    public override HttpMethod Method => HttpMethod.Post;

    public override Delegate Handler { get; } = ([FromRoute] string fromId,
        [FromRoute] string toId,
        [FromServices] UserService userService) =>
    {
        var transaction = userService.SendTransaction(fromId, toId);

        return Results.Ok(transaction);
    };
}