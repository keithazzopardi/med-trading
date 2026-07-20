using Med.Trading.Core.Domain.Entities;
using Med.Trading.Infrastructure.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Med.Trading.Infrastructure.Sql.Configurations;

public class TradingOrderConfiguration : IEntityTypeConfiguration<TradeOrderEntity>
{
    public void Configure(EntityTypeBuilder<TradeOrderEntity> entity)
    {
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).UseIdentityColumn();

        entity.Property(e => e.InstrumentSymbol)
            .IsRequired(false)
            .HasMaxLength(10);

        // InstrumentSymbol references the instrument's unique alternate key rather than its Id.
        // The relationship is optional for orders that have no instrument association.
        // Restrict prevents deleting an instrument while an order references its symbol.
        // This preserves both the TradingOrder row and its InstrumentSymbol value.
        entity.HasOne(e => e.Instrument)
            .WithMany()
            .HasForeignKey(e => e.InstrumentSymbol)
            .HasPrincipalKey(e => e.Symbol)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.Property(x => x.Side)
            .HasConversion(new EnumToStringConverter<OrderSide>())
            .HasMaxLength(15)
            .IsRequired();

        entity.Property(x => x.Status)
            .HasConversion(new EnumToStringConverter<OrderStatus>())
            .HasMaxLength(15)
            .IsRequired();

        entity.Property(x => x.OrderType)
            .HasConversion(new EnumToStringConverter<TraderOrderType>())
            .HasMaxLength(15)
            .IsRequired();
        
        entity.Property(x => x.UserId)
            .IsRequired();

        entity.Property(x => x.PortfolioId)
            .IsRequired();

        entity.Property(x => x.Price)
            .HasPrecision(TradingDbContext.DecimalPrecision, TradingDbContext.DecimalScale)
            .IsRequired();
        
        entity.Property(x => x.CommissionMoney)
            .HasPrecision(TradingDbContext.DecimalPrecision, TradingDbContext.DecimalScale)
            .IsRequired();

        entity.Property(x => x.Quantity)
            .HasPrecision(TradingDbContext.DecimalPrecision, TradingDbContext.DecimalScale)
            .IsRequired();
    }
}
