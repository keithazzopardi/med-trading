using Med.Trading.Core.Domain.Entities;
using Med.Trading.Infrastructure.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Med.Trading.Infrastructure.Sql.Configurations;

public class InstrumentConfiguration : IEntityTypeConfiguration<InstrumentEntity>
{
    public void Configure(EntityTypeBuilder<InstrumentEntity> entity)
    {
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).UseIdentityColumn();

        entity.Property(x => x.Symbol)
            .IsRequired()
            .HasMaxLength(10);
        
        entity.Property(x => x.CurrencyCode)
            .IsRequired()
            .HasMaxLength(5);

        entity.HasIndex(x => x.Symbol)
            .IsUnique();

        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.DefaultProvider)
            .IsRequired()
            .HasMaxLength(50);

        entity.Property(x => x.Price)
            .HasPrecision(TradingDbContext.DecimalPrecision, TradingDbContext.DecimalScale)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(x => x.Volume24h)
            .HasPrecision(TradingDbContext.DecimalPrecision, TradingDbContext.DecimalScale)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(x => x.VolumeLastUpdatedOn)
            .IsRequired(false);

        entity.Property(x => x.UpdatedOn)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow);
        
        entity.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        entity.HasIndex(x => new
        {
            x.UpdatedOn,
            x.IsActive
        }).HasDatabaseName("IX_InstrumentEntity_IsActive");

        entity.HasIndex(x => new
        {
            x.VolumeLastUpdatedOn,
            x.IsActive
        }).HasDatabaseName("IX_InstrumentEntity_VolumeLastUpdatedOn_IsActive");
    }
}
