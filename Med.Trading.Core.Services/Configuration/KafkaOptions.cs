using System.ComponentModel.DataAnnotations;

namespace Med.Trading.Core.Services.Configuration;

public sealed class KafkaOptions
{
    public const string SectionName = "KafkaSettings";
    public const string BootstrapServersSecretName = "Kafka:BootstrapServers";
    public const string SaslUsernameSecretName = "Kafka:SaslUsername";
    public const string SaslPasswordSecretName = "Kafka:SaslPassword";
    public const string SchemaRegistryUrlSecretName = "Kafka:SchemaRegistryUrl";
    public const string SchemaRegistryAuthSecretName = "Kafka:SchemaRegistryAuth";

    [Required]
    public string BootstrapServers { get; set; } = string.Empty;

    [Required]
    public string SchemaRegistryUrl { get; set; } = string.Empty;

    [Required]
    public string SaslUsername { get; set; } = string.Empty;

    [Required]
    public string SaslPassword { get; set; } = string.Empty;

    [Required]
    public string SchemaRegistryAuth { get; set; } = string.Empty;

    public Dictionary<string, string> ConsumerProperties { get; set; } = new();
    public Dictionary<string, string> ProducerProperties { get; set; } = new();
    public Dictionary<string, string> SchemaRegistryProperties { get; set; } = new();
}
