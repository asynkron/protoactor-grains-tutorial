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
            kind: SmartBulbGrainActor.Kind,
            prop: Props.FromProducer(() =>
                new SmartBulbGrainActor(
                    (context, clusterIdentity) => new SmartBulbGrain(context, clusterIdentity)
                )
            )
        );
    
    // create the actor system

    return new ActorSystem(actorSystemConfig)
        .WithServiceProvider(provider)
        .WithRemote(remoteConfig)
        .WithCluster(clusterConfig);
});

builder.Services.AddHostedService<ActorSystemClusterHostedService>();

builder.Services.AddHostedService<HouseSimulator>();

var app = builder.Build();

app.MapGet("/", () => Task.FromResult("Hello, Proto.Cluster!"));

app.MapGet("/smart-bulbs/{identity:alpha}", async (ActorSystem actorSystem, string identity) =>
{
    var smartBulbState = await actorSystem
        .Cluster()
        .GetSmartBulbGrain(identity)
        .GetState(CancellationToken.None);
    
    return smartBulbState.State;
});

app.Run();
