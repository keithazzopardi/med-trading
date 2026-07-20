using Med.Trading.Core.Domain.Entities;
using Med.Trading.Infrastructure.Sql.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Med.Trading.Infrastructure.Sql;

public class TradingDbContext : DbContext
{
    public static int DecimalPrecision { get; } = 18;
    public static int DecimalScale { get; } = 6;

    public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options)
    {
    }
    
#if DEBUG // NOTE: Use this to run migrations
    public TradingDbContext() : base()
    {

    }
        
    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //{
    //    base.OnConfiguring(options);
    //    options.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=trading-db;Trusted_Connection=True;MultipleActiveResultSets=true");
    //}
#endif

    public DbSet<TradeOrderEntity> TradeOrders { get; set; }

    public DbSet<UserHoldingEntity> UserPortfolios { get; set; }
    
    public DbSet<InstrumentEntity> Instruments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Trading");
        
        modelBuilder.ApplyConfiguration(new TradingOrderConfiguration());
        modelBuilder.ApplyConfiguration(new UserPortfolioConfiguration());
        modelBuilder.ApplyConfiguration(new InstrumentConfiguration());
    }
}
