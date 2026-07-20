using Med.Trading.Core.Services.Abstractions;
using Confluent.SchemaRegistry;
using Med.Shared.Services.SecretProvider;
using Med.Trading.Core.Services.Configuration;
using Med.Trading.Core.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace Med.Trading.Core.Services
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddServicesImplementations(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton(TimeProvider.System);

            services.AddOptions<KafkaOptions>()
                .BindConfiguration(KafkaOptions.SectionName)
                .Configure<ISecretProvider>((options, secretProvider) =>
                {
                    options.BootstrapServers = secretProvider.GetSecret(
                        KafkaOptions.BootstrapServersSecretName);
                    options.SaslUsername = secretProvider.GetSecret(
                        KafkaOptions.SaslUsernameSecretName);
                    options.SaslPassword = secretProvider.GetSecret(
                        KafkaOptions.SaslPasswordSecretName);
                    options.SchemaRegistryUrl = secretProvider.GetSecret(
                        KafkaOptions.SchemaRegistryUrlSecretName);
                    options.SchemaRegistryAuth = secretProvider.GetSecret(
                        KafkaOptions.SchemaRegistryAuthSecretName);
                })
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<ISchemaRegistryClient>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<KafkaOptions>>().Value;
                var schemaRegistryProperties = new Dictionary<string, string>(options.SchemaRegistryProperties)
                {
                    ["schema.registry.url"] = options.SchemaRegistryUrl,
                    ["schema.registry.basic.auth.user.info"] = options.SchemaRegistryAuth
                };

                return new CachedSchemaRegistryClient(schemaRegistryProperties);
            });

            services.AddSingleton(typeof(IKafkaProducer<>), typeof(KafkaProducer<>));
            services.AddSingleton(typeof(IKafkaConsumer<>), typeof(KafkaConsumer<>));

            services.AddScoped<IInstrumentService, InstrumentService>();
            services.AddScoped<ITradeOrderService, TradeOrderService>();
            services.AddScoped<IEventPublisher, EventPublisher>();
           

            return services;
        }
    }
}
