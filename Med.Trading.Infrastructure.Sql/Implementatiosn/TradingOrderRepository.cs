using Med.Trading.Core.Domain;
using Med.Trading.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Med.Trading.Infrastructure.Sql.Implementatiosn;

public class TradingOrderRepository(TradingDbContext dbContext) : ITradingOrderRepository
{
    public async Task<IReadOnlyList<TradeOrderEntity>> GetPendingOrdersAsync(
        long userId,
        CancellationToken cancellationToken = default) =>
        await dbContext.TradeOrders
            .AsNoTracking()
            .Where(order => order.UserId == userId && order.Status == OrderStatus.Pending)
            .OrderByDescending(order => order.CreatedOn)
            .ToListAsync(cancellationToken);

    public Task<UserHoldingEntity?> GetHoldingAsync(
        long userId,
        string instrumentSymbol,
        CancellationToken cancellationToken = default) =>
        dbContext.UserPortfolios
            .Include(entity => entity.TradeOrders)
            .SingleOrDefaultAsync(
                entity => entity.UserId == userId && entity.InstrumentSymbol == instrumentSymbol,
                cancellationToken);

    public Task CreateHoldingAsync(
        UserHoldingEntity holding,
        CancellationToken cancellationToken = default)
    {
        dbContext.UserPortfolios.Add(holding);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task AddOrderAsync(
        TradeOrderEntity order,
        CancellationToken cancellationToken = default)
    {
        dbContext.TradeOrders.Add(order);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Dictionary<string, decimal>> GetFilledVolumeByInstrumentAsync(
        DateTime windowStartUtc,
        int take,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(take);

        return dbContext.TradeOrders
            .AsNoTracking()
            .Where(order =>
                order.Status == OrderStatus.Filled &&
                order.FilledOn != null &&
                order.FilledOn >= windowStartUtc &&
                order.InstrumentSymbol != null)
            .GroupBy(order => order.InstrumentSymbol)
            .Select(group => new
            {
                InstrumentSymbol = group.Key!,
                Volume = group.Sum(order => order.Quantity * order.Price)
            })
            .OrderByDescending(group => group.Volume)
            .ThenBy(group => group.InstrumentSymbol)
            .Take(take)
            .ToDictionaryAsync(group => group.InstrumentSymbol, group => group.Volume, cancellationToken);
    }
}
