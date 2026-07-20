using Med.Trading.Core.Domain.Entities;
using Med.Trading.Core.Services.Abstractions;

namespace Med.Trading.Core.Services.Implementations;

internal sealed class EventPublisher(
    IKafkaProducer<OrderCreatedEvent> producer) : IEventPublisher
{
    public async Task PublishAsync(
        TradeOrderEntity entity, CancellationToken cancellationToken = default)
    {
        var orderEvent = new OrderCreatedEvent
        {
            OrderId = entity.Id,
            UserId = entity.UserId,
            InstrumentSymbol = entity.InstrumentSymbol ?? string.Empty,
            Side = (int)entity.Side,
            Quantity = entity.Quantity,
            Price = entity.Price,
            CreatedOn = entity.CreatedOn
        };

        await producer.ProduceAsync(entity.Id.ToString(), orderEvent, cancellationToken);
    }
}
