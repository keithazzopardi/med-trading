namespace Med.Trading.Core.Services.Abstractions;

public interface IKafkaProducer<in TEvent> : IDisposable
    where TEvent : class
{
    Task ProduceAsync(string key, TEvent value, CancellationToken cancellationToken = default);
}
