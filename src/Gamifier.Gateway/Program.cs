using System.Reflection;
using Gamifier.Gateway;
using Gamifier.Gateway.Services;
using Gamifier.GrainInterfaces;
using Gamifier.Hangman.Core;
using Microsoft.OpenApi.Models;
using Orleans.Configuration;
using Orleans.Providers.MongoDB.Configuration;
using Orleans.Runtime;
using Orleans.Streaming.Grains.Streams;
using Orleans.Streams;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseOrleansClient(clientBuilder =>
{
    var clusterName = builder.Environment.IsDevelopment() ? "dev" : "Gamifier";

    clientBuilder.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = clusterName;
        options.ServiceId = "Gamifier.Gateway";
    });

    clientBuilder.UseMongoDBClient(builder.Configuration.GetConnectionString("MongoDb"));
    clientBuilder.UseMongoDBClustering(options =>
    {
        options.ClientName = "Gamifier.Gateway";
        options.DatabaseName = "Cluster";
    });
});

// get assembly version
var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddGrpc().AddJsonTranscoding();
    builder.Services.AddGrpcSwagger();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc($"v{assemblyVersion}", new OpenApiInfo
        {
            Title = "Gamifier.Gateway gRPC API",
            Version = $"v{assemblyVersion}"
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
        options.IncludeGrpcXmlComments(xmlPath, includeControllerXmlComments: true);
    });
}
else
{
    builder.Services.AddGrpc();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint($"/swagger/v{assemblyVersion}/swagger.json",
            $"Gamifier.Gateway gRPC API v{assemblyVersion}");
    });
}

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.StartAsync();
// get cluster client
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var client = app.Services.GetRequiredService<IClusterClient>();

var streamProvider = client.GetStreamProvider(Constants.PubSubProvider);

var gameId = Guid.NewGuid();
var stream = streamProvider.GetStream<HangmanMessage>(StreamId.Create(StreamNamespaces.HangmanGame, gameId));

var gameGrain = client.GetGrain<IHangmanGameGrain>(gameId);

await stream.SubscribeAsync(async (message, token) =>
{
    logger.LogInformation("Received message (token: {token}): {message}", token, message);
});

logger.LogInformation("Sending start game message");
var word = "test";
var callerId = Guid.NewGuid();
await gameGrain.StartGameAsync(word, callerId);


logger.LogInformation("Sending guess message");

var guess = 't';

await gameGrain.GuessAsync(guess, callerId);

logger.LogInformation("Sending guess message");

guess = 'e';

await gameGrain.GuessAsync(guess, callerId);

logger.LogInformation("Sending guess message");


guess = 's';

await gameGrain.GuessAsync(guess, callerId);

logger.LogInformation("Sending guess message");

guess = 't';

await gameGrain.GuessAsync(guess, callerId);

Console.WriteLine("Press any key to exit...");
Console.ReadKey();