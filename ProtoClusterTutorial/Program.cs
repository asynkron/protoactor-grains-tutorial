using Proto;
using Proto.Cluster;
using ProtoClusterTutorial;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddActorSystem();
builder.Services.AddHostedService<ActorSystemClusterHostedService>();
builder.Services.AddHostedService<SmartBulbSimulator>();

var app = builder.Build();

app.MapGet("/", () => Task.FromResult("Hello, Proto.Cluster!"));

app.MapGet("/smart-bulbs/{identity}", async (ActorSystem actorSystem, string identity) =>
{
    return await actorSystem
        .Cluster()
        .GetSmartBulbGrain(identity)
        .GetState(CancellationToken.None);
});

app.Run();