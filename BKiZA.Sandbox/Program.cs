using System.Reflection;
using BKiZA.ProofOfStake;
using BKiZA.ProofOfWork;
using BKiZA.Sandbox;
using BKiZA.Shared;
using BKiZA.Shared.Infrastructure;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes;
using BKiZA.Shared.Nodes.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var proofOfWorkAssemblies = new[] { Assembly.Load("BKiZA.Shared"), Assembly.Load("BKiZA.ProofOfWork") };
var proofOfStakeAssemblies = new[] { Assembly.Load("BKiZA.Shared"), Assembly.Load("BKiZA.ProofOfStake") };

builder.Services
    .AddEndpointsApiExplorer()
    .AddNetwork()
    .AddNodes();

var networkType = builder.Configuration.GetValue<string>("NetworkType");

if (networkType == "proof_of_work")
{
    builder.Services.AddProofOfWork();
    builder.Services.AddInfrastructure(proofOfWorkAssemblies);
    builder.Services.AddHostedService<ProofOfWorkService>();
}

if (networkType == "proof_of_stake")
{
    builder.Services.AddProofOfStake();
    builder.Services.AddInfrastructure(proofOfStakeAssemblies);
    builder.Services.AddHostedService<ProofOfStakeService>();
}

builder.Services.AddSwaggerGen(swagger =>
{
    swagger.CustomSchemaIds(x => x.FullName);
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BKiZA API",
        Version = "v1"
    });
});

var app = builder.Build();

networkType = app.Configuration.GetValue<string>("NetworkType");

if (networkType == "proof_of_work")
{
    app.MapModuleEndpoints(proofOfWorkAssemblies);
}

if (networkType == "proof_of_stake")
{
    app.MapModuleEndpoints(proofOfStakeAssemblies);
}

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "BKiZA API";
    options.RoutePrefix = "docs";
    options.SwaggerEndpoint($"/swagger/v1/swagger.json", "BKiZA API");
});

var networkBroker = app.Services.GetRequiredService<NetworkBroker>();
var userNetworkStorage = app.Services.GetRequiredService<INetworkStorage<User>>();

var startUsers = userNetworkStorage.Scan();
startUsers.RemoveAll(User => User.NodeId == "A");
    
    startUsers.ForEach(user
    => networkBroker.Publish(new UserNodeAdded(user)));

await app.RunAsync();