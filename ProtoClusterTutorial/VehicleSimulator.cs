using Proto;
using Proto.Cluster;

namespace ProtoClusterTutorial;

public class VehicleSimulator : BackgroundService
{
    private readonly ActorSystem _actorSystem;

    public VehicleSimulator(ActorSystem actorSystem)
    {
        _actorSystem = actorSystem;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            var vehicleIdentity = $"vehicle-{random.Next(10)}";

            var vehicleActorClient = _actorSystem
                .Cluster()
                .GetVehicleActor(vehicleIdentity);

            await vehicleActorClient.ChangeVehiclePosition(
                request: new ChangeVehiclePositionRequest
                {
                    Position = new Position
                    {
                        Latitude = random.NextDouble() * 100,
                        Longitude = random.NextDouble() * 100
                    }
                },
                ct: stoppingToken
            );

            await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
        }
    }
}