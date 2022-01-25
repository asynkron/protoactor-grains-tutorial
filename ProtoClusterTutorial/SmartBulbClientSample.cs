using Proto;
using Proto.Cluster;

namespace ProtoClusterTutorial;

public class SmartBulbClientSample
{
    private readonly ActorSystem _actorSystem;

    public SmartBulbClientSample(ActorSystem actorSystem)
    {
        _actorSystem = actorSystem;
    }
    
    public async Task TurnLightOnInKitchen(CancellationToken ct)
    {
        var smartBulbGrainClient = _actorSystem
            .Cluster()
            .GetSmartBulbGrain("kitchen");

        await smartBulbGrainClient.TurnOn(ct);
    }
}