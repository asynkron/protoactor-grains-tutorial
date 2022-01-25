using Proto;
using Proto.Cluster;

namespace ProtoClusterTutorial;

public class SmartBulbGrain : SmartBulbGrainBase
{
    private readonly ClusterIdentity _clusterIdentity;
    
    private enum SmartBulbState { Unknown, On, Off }
    private SmartBulbState _state = SmartBulbState.Unknown;
    
    public SmartBulbGrain(IContext context, ClusterIdentity clusterIdentity) : base(context)
    {
        _clusterIdentity = clusterIdentity;

        Console.WriteLine($"{_clusterIdentity.Identity}: created");
    }

    public override async Task TurnOn()
    {
        if (_state != SmartBulbState.On)
        {
            Console.WriteLine($"{_clusterIdentity.Identity}: turning smart bulb on");
            
            _state = SmartBulbState.On;
            
            await NotifyHouse();
        }
    }

    public override async Task TurnOff()
    {
        if (_state != SmartBulbState.Off)
        {
            Console.WriteLine($"{_clusterIdentity.Identity}: turning smart bulb off");
            
            _state = SmartBulbState.Off;
            
            await NotifyHouse();
        }
    }
    
    private async Task NotifyHouse()
    {
        await Context
            .GetSmartHouseGrain("my-house")
            .SmartBulbStateChanged(
                new SmartBulbStateChangedRequest
                {
                    SmartBulbIdentity = _clusterIdentity.Identity,
                    IsOn = _state == SmartBulbState.On
                },
                CancellationToken.None
            );
    }
    
    public override Task<GetSmartBulbStateResponse> GetState()
    {
        return Task.FromResult(new GetSmartBulbStateResponse
        {
            State = _state.ToString()
        });
    }
}