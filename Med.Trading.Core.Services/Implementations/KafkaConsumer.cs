using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Med.Trading.Core.Services.Abstractions;
using Med.Trading.Core.Services.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Med.Trading.Core.Services.Implementations;

public sealed class KafkaConsumer<TEvent> : IKafkaConsumer<TEvent>
    where TEvent : class
{
    private readonly IConsumer<string, TEvent> consumer;

    public KafkaConsumer(
        IOptions<KafkaOptions> options,
        ISchemaRegistryClient schemaRegistryClient,
        IHostEnvironment hostEnvironment)
    {
        var kafkaOptions = options.Value;
        var consumerProperties = new Dictionary<string, string>(kafkaOptions.ConsumerProperties)
        {
            ["bootstrap.servers"] = kafkaOptions.BootstrapServers,
            ["sasl.username"] = kafkaOptions.SaslUsername,
            ["sasl.password"] = kafkaOptions.SaslPassword,
            ["group.id"] = AppDomain.CurrentDomain.FriendlyName,
            ["client.id"] = AppDomain.CurrentDomain.FriendlyName
        };

        var deserializer = new JsonDeserializer<TEvent>(schemaRegistryClient).AsSyncOverAsync();

        consumer = new ConsumerBuilder<string, TEvent>(new ConsumerConfig(consumerProperties))
            .SetValueDeserializer(deserializer)
            .Build();
    }

    public void Subscribe() => consumer.Subscribe(typeof(TEvent).Name);

    public TEvent? Consume(CancellationToken cancellationToken)
    {
        var result = consumer.Consume(cancellationToken);
        return result.Message.Value;
    }

    public void Dispose()
    {
        consumer.Close();
        consumer.Dispose();
    }
}
