using k8s;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Cluster.Kubernetes;
using Proto.Cluster.Partition;
using Proto.Cluster.Testing;
using Proto.DependencyInjection;
using Proto.Remote;
using Proto.Remote.GrpcNet;

namespace ProtoClusterTutorial;

public static class ActorSystemConfiguration
{
    public static void AddActorSystem(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton(provider =>
        {
            // actor system configuration

            var actorSystemConfig = ActorSystemConfig
                .Setup();

            // remote configuration

            var remoteConfig = GrpcNetRemoteConfig
                    .BindToAllInterfaces(advertisedHost: configuration["ProtoActor:AdvertisedHost"])
                    .WithProtoMessages(MessagesReflection.Descriptor);

            // cluster configuration
            var clusterConfig = ClusterConfig
                .Setup(
                    clusterName: "ProtoClusterTutorial",
                    
                    clusterProvider: new TestProvider(new TestProviderOptions(), new InMemAgent()), 
                    // if running in Kubernetes, use this instead
                    // clusterProvider: new KubernetesProvider(),
                    
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