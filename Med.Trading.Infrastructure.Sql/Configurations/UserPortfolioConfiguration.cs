using Med.Trading.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Med.Trading.Infrastructure.Sql.Configurations;

public class UserPortfolioConfiguration : IEntityTypeConfiguration<UserHoldingEntity>
{
    public void Configure(EntityTypeBuilder<UserHoldingEntity> entity)
    {
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).UseIdentityColumn();

        entity.Property(x => x.UserId)
            .IsRequired();

        entity.Property(x => x.InstrumentSymbol)
            .IsRequired(false)
            .HasMaxLength(10);

        entity.Property(x => x.AvailableQuantity)
            .HasPrecision(TradingDbContext.DecimalPrecision, TradingDbContext.DecimalScale)
            .HasDefaultValue(0)
            .IsRequired();

        entity.HasIndex(x => new { x.UserId, x.InstrumentSymbol })
            .IsUnique()
            .HasDatabaseName("UX_UserPortfolio_UserId_InstrumentSymbol");

        entity.HasOne(x => x.Instrument)
            .WithMany()
            .HasForeignKey(x => x.InstrumentSymbol)
            .HasPrincipalKey(x => x.Symbol)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(x => x.TradeOrders)
            .WithOne(x => x.Holding)
            .HasForeignKey(x => x.PortfolioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
