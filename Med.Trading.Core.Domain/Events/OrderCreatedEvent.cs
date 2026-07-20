namespace Med.Trading.Core.Services.Abstractions;

public class OrderCreatedEvent
{
    public int OrderId { get; set; }
    public long UserId { get; set; }

    public string InstrumentSymbol { get; set; } = string.Empty;
    public int Side { get; set; }
    public decimal Quantity { get; set; }

    public decimal Price { get; set; }
    public DateTime CreatedOn { get; set; }
}
