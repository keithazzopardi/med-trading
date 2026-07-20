using Med.Trading.Core.Domain.Entities;
using Med.Shared;

namespace Med.Trading.Core.Services.Abstractions;

public interface ITradeOrderService
{
    Task<IReadOnlyList<TradeOrderEntity>> GetPendingOrdersAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<OperationResponse> CreateOrderAsync(
        CreateTradeOrderRequest request,
        long userId,
        CancellationToken cancellationToken = default);
}
