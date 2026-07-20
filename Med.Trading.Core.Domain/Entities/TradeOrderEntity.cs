namespace Med.Trading.Core.Domain.Entities;

public class TradeOrderEntity
{
    public int Id { get; set; }
    
    public string? InstrumentSymbol { get; set; }

    /// <summary>
    /// The associated instrument, if it is still present in the instrument catalogue.
    /// </summary>
    public InstrumentEntity? Instrument { get; set; }
    public OrderSide Side { get; set; }
    public TraderOrderType OrderType { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    
    public long UserId { get; set; }
    public long PortfolioId { get; set; }
    public UserHoldingEntity Holding { get; set; } = null!;
    public decimal CommissionMoney { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? FilledOn { get; set; }
}

public enum OrderSide : int
{
    Unknown = 0,
    Buy = 1,
    Sell = 2
}

public enum TraderOrderType : int
{
    Unknown = 0,
    Market = 1,
    StopLimit = 2,
    LossLimit = 3
}

public enum OrderStatus
{
    Pending = 0,
    Filled = 1,
    Rejected = 2,
    Cancelled = 3
}
