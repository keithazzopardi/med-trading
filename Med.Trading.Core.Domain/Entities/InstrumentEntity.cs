namespace Med.Trading.Core.Domain.Entities;

public class InstrumentEntity
{
    public long Id { get; set; }
    
    public string Symbol { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DefaultProvider { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    public decimal Volume24h { get; set; }

    /// <summary>
    /// Indicates when <see cref="Volume24h"/> was last calculated.
    /// A null value means the instrument was not included in the latest top-volume refresh.
    /// </summary>
    public DateTime? VolumeLastUpdatedOn { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime UpdatedOn { get; set; }

    public void UpdateVolume24Hour(decimal volume, DateTime calculatedAtUtc)
    {
        Volume24h = volume;
        VolumeLastUpdatedOn = calculatedAtUtc;    
    }
}
