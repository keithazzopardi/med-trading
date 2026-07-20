using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Med.Trading.Core.Services.Abstractions;
using Med.Trading.Core.Services.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Med.Trading.Core.Services.Implementations;

public sealed class KafkaProducer<TEvent> : IKafkaProducer<TEvent>
    where TEvent : class
{
    private readonly IProducer<string, TEvent> producer;
    private readonly KafkaOptions options;

    public KafkaProducer(
        IOptions<KafkaOptions> options,
        ISchemaRegistryClient schemaRegistryClient,
        IHostEnvironment hostEnvironment)
    {
        this.options = options.Value;

        var producerProperties = new Dictionary<string, string>(this.options.ProducerProperties)
        {
            ["bootstrap.servers"] = this.options.BootstrapServers,
            ["sasl.username"] = this.options.SaslUsername,
            ["sasl.password"] = this.options.SaslPassword,
            ["client.id"] = AppDomain.CurrentDomain.FriendlyName
        };

        var serializer = new JsonSerializer<TEvent>(
            schemaRegistryClient,
            new JsonSerializerConfig
            {
                AutoRegisterSchemas = true
            });

        producer = new ProducerBuilder<string, TEvent>(new ProducerConfig(producerProperties))
            .SetValueSerializer(serializer)
            .Build();
    }

    public async Task ProduceAsync(
        string key, TEvent value, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        await producer.ProduceAsync(
            typeof(TEvent).Name,
            new Message<string, TEvent> { Key = key, Value = value },
            cancellationToken);
    }

    public void Dispose() => producer.Dispose();
}
