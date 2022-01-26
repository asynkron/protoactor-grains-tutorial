using Proto;
using Proto.Cluster;
using Proto.Cluster.Partition;
using Proto.Cluster.Testing;
using Proto.DependencyInjection;
using Proto.Remote;
using Proto.Remote.GrpcCore;

namespace ProtoClusterTutorial;

public static class ActorSystemConfiguration
{
    public static void AddActorSystem(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(provider =>
        {
            // actor system configuration

            var actorSystemConfig = ActorSystemConfig
                .Setup();
    
            // remote configuration

            var remoteConfig = GrpcCoreRemoteConfig
                .BindToLocalhost()
                .WithProtoMessages(MessagesReflection.Descriptor);
    
            // cluster configuration

            var clusterConfig = ClusterConfig
                .Setup(
                    clusterName: "ProtoClusterTutorial",
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
                )
                .WithClusterKind(
                    kind: SmartHouseGrainActor.Kind,
                    prop: Props.FromProducer(() =>
                        new SmartHouseGrainActor(
                            (context, clusterIdentity) => new SmartHouseGrain(context, clusterIdentity)
                        )
                    )
                );
    
            // create the actor system

            return new ActorSystem(actorSystemConfig)
                .WithServiceProvider(provider)
                .WithRemote(remoteConfig)
                .WithCluster(clusterConfig);
        });
    }
}