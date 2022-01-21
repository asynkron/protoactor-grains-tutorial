using Proto;
using Proto.Cluster;

namespace ProtoClusterTutorial;

public class VehicleActor : VehicleActorBase
{
    private readonly ClusterIdentity _clusterIdentity;
    
    private readonly List<Position> _positionHistory = new();
    
    public VehicleActor(IContext context, ClusterIdentity clusterIdentity) : base(context)
    {
        _clusterIdentity = clusterIdentity;
    }

    public override Task<ChangeVehiclePositionResponse> ChangeVehiclePosition(ChangeVehiclePositionRequest request)
    {
        _positionHistory.Add(request.Position);

        Console.WriteLine($"{_clusterIdentity} position changed to: {request}");

        return Task.FromResult(new ChangeVehiclePositionResponse());
    }

    public override Task<GetVehiclePositionsHistoryResponse> GetVehiclePositionsHistory(GetVehiclePositionsHistoryRequest request)
    {
        var response = new GetVehiclePositionsHistoryResponse();

        response.Positions.AddRange(_positionHistory);

        return Task.FromResult(response);
    }
}