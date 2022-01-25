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
    }

    public override Task TurnOn()
    {
        if (_state != SmartBulbState.On)
        {
            _state = SmartBulbState.On;
            Console.WriteLine($"{_clusterIdentity.Identity}: turning smart bulb on");
        }
    
        return Task.CompletedTask;
    }

    public override Task TurnOff()
    {
        if (_state != SmartBulbState.Off)
        {
            _state = SmartBulbState.Off;
            Console.WriteLine($"{_clusterIdentity.Identity}: turning smart bulb off");
        }
    
        return Task.CompletedTask;
    }
    
    public override Task<GetSmartBulbStateResponse> GetState()
    {
        return Task.FromResult(new GetSmartBulbStateResponse
        {
            State = _state.ToString()
        });
    }
}