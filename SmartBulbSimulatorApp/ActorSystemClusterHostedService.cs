using Microsoft.Extensions.Hosting;
using Proto;
using Proto.Cluster;

namespace SmartBulbSimulatorApp;

public class ClusterClientHostedService : IHostedService
{
    private readonly ActorSystem _actorSystem;

    public ClusterClientHostedService(ActorSystem actorSystem)
    {
        _actorSystem = actorSystem;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Starting a cluster client");

        await _actorSystem
            .Cluster()
            .StartClientAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Shutting down a cluster client");

        await _actorSystem
            .Cluster()
            .ShutdownAsync();
    }
}