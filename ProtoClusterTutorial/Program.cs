using Proto;
using Proto.Cluster;
using Proto.Cluster.Partition;
using Proto.Cluster.Testing;
using Proto.DependencyInjection;
using Proto.Remote.GrpcCore;
using ProtoClusterTutorial;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(provider =>
{
    // actor system configuration

    var actorSystemConfig = ActorSystemConfig
        .Setup();
    
    // remote configuration
    
    var remoteConfig = GrpcCoreRemoteConfig.BindToLocalhost();
    
    // cluster configuration

    var clusterConfig = ClusterConfig
        .Setup(
            clusterName: "RealtimeMapCluster",
            clusterProvider: new TestProvider(new TestProviderOptions(), new InMemAgent()),
            identityLookup: new PartitionIdentityLookup()
        )
        .WithClusterKind(
            kind: "VehicleActor",
            prop: Props.FromProducer(() =>
                new VehicleActorActor(
                    (context, clusterIdentity) => new VehicleActor(context, clusterIdentity)
                )
            )
        );
    
    // create the actor system

    return new ActorSystem(actorSystemConfig)
        .WithServiceProvider(provider)
        .WithRemote(remoteConfig)
        .WithCluster(clusterConfig);
});

builder.Services.AddHostedService<ActorSystemHostedService>();

builder.Services.AddHostedService<VehicleSimulator>();

var app = builder.Build();

app.MapGet("/", () => Task.FromResult("hello, world"));

app.Run();
