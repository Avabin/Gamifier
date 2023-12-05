// See https://aka.ms/new-console-template for more information

using Gamifier.GrainInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Providers.MongoDB.Configuration;
using Orleans.Streaming.Grains.Extensions;
using Serilog;

// get current directory
var currentDirectory = Directory.GetCurrentDirectory();
var hostConfiguration = new ConfigurationManager();
hostConfiguration
    .SetBasePath(currentDirectory)
    .AddEnvironmentVariables("DOTNET_")
    .AddEnvironmentVariables("ASPNETCORE_")
    .AddCommandLine(args);
var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings()
{
    ApplicationName = "Gamifier.Silo",
    Args = args,
    DisableDefaults = true,
    Configuration = hostConfiguration,
    ContentRootPath = currentDirectory,
});
    
var environment = builder.Environment;


builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
    
builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger());
    
builder.UseOrleans(siloBuilder =>
{
    siloBuilder.Configure<ClusterOptions>(options =>
    {
        var clusterName = builder.Environment.IsDevelopment() ? "dev" : "Gamifier";
        options.ClusterId = clusterName;
        options.ServiceId = "Gamifier.Silo";
    });
    siloBuilder.AddReminders()
        .UseMongoDBClient(builder.Configuration.GetConnectionString("MongoDB"))
        .UseMongoDBReminders(options =>
        {
            options.ClientName = "Gamifier.Silo";
            options.DatabaseName = "Reminders";
        })
        .UseMongoDBClustering(options =>
        {
            options.ClientName = "Gamifier.Silo";
            options.DatabaseName = "Cluster";
            options.Strategy = MongoDBMembershipStrategy.SingleDocument;
        })
        .AddMongoDBGrainStorageAsDefault(options =>
        {
            options.ClientName = "Gamifier.Silo";
            options.DatabaseName = "Default";
        })
        .AddMongoDBGrainStorage(Constants.PubSubStorage, options =>
        {
            options.ClientName = "Gamifier.Silo";
            options.DatabaseName = "PubSub";
        })
        .AddGrainsStreams(Constants.PubSubProvider, 3, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(3));
});

var host = builder.Build();


await host.RunAsync();