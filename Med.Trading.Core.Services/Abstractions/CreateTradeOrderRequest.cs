using Med.Trading.Core.Domain.Entities;

namespace Med.Trading.Core.Services.Abstractions;

public record CreateTradeOrderRequest(
    string Symbol,
    decimal Quantity,
    decimal Price,
    OrderSide Side,
    TraderOrderType OrderType);
