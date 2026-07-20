using Med.Shared;
using Med.Trading.Core.Domain;
using Med.Trading.Core.Domain.Entities;
using Med.Trading.Core.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Med.Trading.Core.Services.Implementations;

public class TradeOrderService(
    ITradingOrderRepository tradingOrderRepository,
    IInstrumentRepository instrumentRepository,
    IEventPublisher eventPublisher,
    ILogger<TradeOrderService> logger) : ITradeOrderService
{
    public Task<IReadOnlyList<TradeOrderEntity>> GetPendingOrdersAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return tradingOrderRepository.GetPendingOrdersAsync(userId, cancellationToken);
    }

    public async Task<OperationResponse> CreateOrderAsync(
        CreateTradeOrderRequest request,
        long userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
        if (request.Quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(request), "Quantity must be positive.");

        var instrumentSymbol = request.Symbol?.Trim();
        if (string.IsNullOrWhiteSpace(instrumentSymbol))
            throw new ArgumentException("Instrument symbol is required.", nameof(request));

        var instrument = await instrumentRepository.GetBySymbolAsync(instrumentSymbol, cancellationToken);
        if (instrument is null)
            return OperationResponse.CreateFailure((int)ErrorCodes.InstrumentNotFound);

        var holding = await tradingOrderRepository.GetHoldingAsync(userId, instrumentSymbol, cancellationToken);
        TradeOrderEntity createdOrder;

        if (holding is null)
        {
            holding = UserHoldingEntity.Initialize(userId, instrument);
            createdOrder =  holding.AddOrder(
                instrument,
                request.Side,
                request.OrderType,
                request.Quantity,
                request.Price,
                commissionMoney: 0);
            await tradingOrderRepository.CreateHoldingAsync(holding, cancellationToken);
        }
        else
        {
            createdOrder =  holding.AddOrder(
                instrument,
                request.Side,
                request.OrderType,
                request.Quantity,
                request.Price,
                commissionMoney: 0);
            await tradingOrderRepository.AddOrderAsync(createdOrder, cancellationToken);
        }

        logger.LogInformation("Order {OrderId} has been registered for user {UserId}", createdOrder.Id, userId);
        await eventPublisher.PublishAsync(createdOrder, cancellationToken);
        return OperationResponse.CreateSuccess();
    }
}
