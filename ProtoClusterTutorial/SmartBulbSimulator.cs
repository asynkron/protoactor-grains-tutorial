using Proto;
using Proto.Cluster;

namespace ProtoClusterTutorial;

public class SmartBulbSimulator : BackgroundService
{
    private readonly ActorSystem _actorSystem;

    public SmartBulbSimulator(ActorSystem actorSystem)
    {
        _actorSystem = actorSystem;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();

        var lightBulbs = new[] { "living_room_1", "living_room_2", "bedroom", "kitchen" };

        while (!stoppingToken.IsCancellationRequested)
        {
            var randomIdentity = lightBulbs[random.Next(lightBulbs.Length)];

            var smartBulbGrainClient = _actorSystem
                .Cluster()
                .GetSmartBulbGrain(randomIdentity);

            if (random.Next(2) > 0)
            {
                Console.WriteLine($"smart bulb simulator: turning on smart bulb '{randomIdentity}'");
                
                await smartBulbGrainClient.TurnOn(stoppingToken);
            }
            else
            {
                Console.WriteLine($"smart bulb simulator: turning off smart bulb '{randomIdentity}'");
                
                await smartBulbGrainClient.TurnOff(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
        }
    }
}