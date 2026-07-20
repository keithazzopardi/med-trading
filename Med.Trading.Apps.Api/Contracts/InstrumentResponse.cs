namespace Med.Trading.Apps.Api.Contracts;

public record InstrumentResponse
{
    public long Id { get; init; }
    public string Symbol { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string DefaultProvider { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal Volume24h { get; init; }
    public bool IsActive { get; init; }
    public DateTime UpdatedOn { get; init; }
}
