using Hangfire;
using Hangfire.SqlServer;
using Med.Shared;
using Med.Shared.Services.SecretProvider;
using Med.Trading.Core.Consumers;
using Med.Trading.Core.Services;
using Med.Trading.Core.Services.Abstractions;
using Med.Trading.Infrastructure.Sql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSharedImplementations();
builder.Services.AddServicesImplementations();
builder.Services.AddSqlImplementations();
builder.Services.AddHostedService<OrderProviderConsumer>();
builder.Services.AddHangfire((serviceProvider, configuration) =>
{
    var secretProvider = serviceProvider.GetRequiredService<ISecretProvider>();
    var connectionString = secretProvider.GetSecret("ConnectionStrings:SqlConnection");

    configuration
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
        {
            SchemaName = "Hangfire",
            PrepareSchemaIfNecessary = true
        });
});
builder.Services.AddHangfireServer();

using var host = builder.Build();

host.Services.GetRequiredService<IRecurringJobManager>().AddOrUpdate<IInstrumentService>(
    "instrument-volume24h-refresh",
    service => service.RefreshVolume24hAsync(),
    Cron.Daily,
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

await host.RunAsync();
