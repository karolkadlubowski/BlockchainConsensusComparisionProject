using System;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Infrastructure.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKiZA.Shared.Endpoints;

public class GetMetricsHistoryEndpoint : ModuleEndpoint
{
    public override string Route => "metrics/history";
    public override HttpMethod Method => HttpMethod.Get;

    public override Delegate Handler { get; } = ([FromServices] IMetricsHistory metricsHistory)
        => Results.Ok(metricsHistory.GetHistory());
}