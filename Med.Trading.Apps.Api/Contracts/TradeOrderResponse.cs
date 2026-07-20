namespace Med.Trading.Apps.Api.Contracts;

public record TradeOrderResponse
{
    public int Id { get; init; }
    public string? InstrumentSymbol { get; init; }
    public string Side { get; init; } = string.Empty;
    public string OrderType { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
    public long UserId { get; init; }
    public long PortfolioId { get; init; }
    public decimal CommissionMoney { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedOn { get; init; }
    public DateTime? FilledOn { get; init; }
}
