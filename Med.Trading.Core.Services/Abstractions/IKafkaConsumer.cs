namespace Med.Trading.Core.Services.Abstractions;

public interface IKafkaConsumer<TEvent> : IDisposable
    where TEvent : class
{
    void Subscribe();
    TEvent? Consume(CancellationToken cancellationToken);
}
