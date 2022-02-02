using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Cluster.Partition;
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
                .BindToLocalhost(provider
                    .GetRequiredService<IConfiguration>()
                    .GetValue<int>("ProtoRemotePort")
                )
                .WithProtoMessages(MessagesReflection.Descriptor);
    
            // cluster configuration

            var clusterConfig = ClusterConfig
                .Setup(
                    clusterName: "ProtoClusterTutorial",
                    clusterProvider: new ConsulProvider(new ConsulProviderConfig()),
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