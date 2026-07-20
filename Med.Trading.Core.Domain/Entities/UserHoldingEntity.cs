namespace Med.Trading.Core.Domain.Entities;

public class UserHoldingEntity
{
    public long Id { get; set; }
    
    public long UserId { get; set; }
    
    public string? InstrumentSymbol { get; set; }

    public InstrumentEntity? Instrument { get; set; }
    
    public decimal AvailableQuantity { get; set; }
    
    public List<TradeOrderEntity> TradeOrders { get; set; } = [];
    
    public TradeOrderEntity AddOrder(
        InstrumentEntity instrument,
        OrderSide side,
        TraderOrderType orderType,
        decimal quantity,
        decimal price,
        decimal commissionMoney)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        if (commissionMoney < 0)
            throw new ArgumentOutOfRangeException(nameof(commissionMoney), "Commission cannot be negative.");
        
        // NOTE: Can you have a sell order without having enough quantity? Possibly consider to validate this.
        
        var tradeOrder = new TradeOrderEntity
        {
            InstrumentSymbol = instrument.Symbol,
            Side = side,
            OrderType = orderType,
            Quantity = quantity,
            Price = price,
            UserId = UserId,
            Holding = this,
            CommissionMoney = commissionMoney,
            Status = OrderStatus.Pending,
            CreatedOn = DateTime.UtcNow
        };

        TradeOrders.Add(tradeOrder);
        
        return tradeOrder;
    }

    public bool MarkOrderAsFilled(long orderId)
    {
        var order = TradeOrders.SingleOrDefault(order => order.Id == orderId);
        if (order is null || order.Status != OrderStatus.Pending)
            return false;

        if (order.Side == OrderSide.Sell && AvailableQuantity < order.Quantity)
            return false;

        AvailableQuantity += order.Side == OrderSide.Buy
            ? order.Quantity
            : -order.Quantity;
        
        order.Status = OrderStatus.Filled;
        order.FilledOn = DateTime.UtcNow;
        return true;
    }

    public static UserHoldingEntity Initialize(long userId, InstrumentEntity instrument)
    {
        return new UserHoldingEntity
        {
            UserId = userId,
            InstrumentSymbol = instrument.Symbol,
            AvailableQuantity = 0
        };
    }
}
