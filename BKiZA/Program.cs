using BKiZA.Shared;
using BKiZA.Shared.Network;
using BKiZA.Shared.Nodes;
using BKiZA.Shared.Nodes.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => services
        .AddInfrastructure()
        .AddNetwork()
        .AddNodes())
    .Build();

const string RootId = "A";

var networkBroker = host.Services.GetRequiredService<NetworkBroker>();
var userNetworkStorage = host.Services.GetRequiredService<INetworkStorage<User>>();
userNetworkStorage.Scan()
    .ForEach(user => networkBroker.Publish(new UserNodeAdded(user)));

await host.RunAsync();