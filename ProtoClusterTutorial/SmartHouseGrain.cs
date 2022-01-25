using Proto;
using Proto.Cluster;

namespace ProtoClusterTutorial;

public class SmartHouseGrain : SmartHouseGrainBase
{
    private readonly ClusterIdentity _clusterIdentity;

    private readonly SortedSet<string> _turnedOnSmartBulbs = new();

    public SmartHouseGrain(IContext context, ClusterIdentity clusterIdentity) : base(context)
    {
        _clusterIdentity = clusterIdentity;
    }

    public override Task SmartBulbStateChanged(SmartBulbStateChangedRequest request)
    {
        if (request.IsOn)
        {
            _turnedOnSmartBulbs.Add(request.SmartBulbIdentity);
        }
        else
        {
            _turnedOnSmartBulbs.Remove(request.SmartBulbIdentity);
        }

        Console.WriteLine($"{_clusterIdentity.Identity}: {_turnedOnSmartBulbs.Count} smart bulbs are on");

        return Task.CompletedTask;
    }
}