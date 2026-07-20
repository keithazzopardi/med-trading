using Med.Trading.Core.Domain.Entities;

namespace Med.Trading.Core.Services.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync(TradeOrderEntity entity, CancellationToken cancellationToken = default);
}
