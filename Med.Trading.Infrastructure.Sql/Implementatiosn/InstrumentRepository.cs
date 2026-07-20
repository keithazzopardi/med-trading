using Med.Trading.Core.Domain;
using Med.Trading.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Med.Trading.Infrastructure.Sql.Implementatiosn;

public class InstrumentRepository(TradingDbContext dbContext) : IInstrumentRepository
{
    public Task<List<InstrumentEntity>> GetTrendingInsturmentsAsync(
        int count, int days, CancellationToken cancellationToken)
    {
        var weekStartUtc = DateTime.UtcNow.AddDays(-days);

        return dbContext.Instruments
            .AsNoTracking()
            .Where(instrument => instrument.VolumeLastUpdatedOn >= weekStartUtc && instrument.IsActive)
            .OrderByDescending(instrument => instrument.Volume24h)
            .ThenByDescending(instrument => instrument.VolumeLastUpdatedOn)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public Task<List<InstrumentEntity>> GetTopInstrumentsByVolumeAsync(
        int count, CancellationToken cancellationToken)
    {
        return dbContext.Instruments
            .AsNoTracking()
            .Where(instrument => instrument.IsActive)
            .OrderByDescending(instrument => instrument.Volume24h)
            .ThenBy(instrument => instrument.Symbol)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public Task<InstrumentEntity?> GetBySymbolAsync(
        string symbol, CancellationToken cancellationToken)
    {
        return dbContext.Instruments
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Symbol == symbol && x.IsActive, cancellationToken);
    }

    public Task<List<InstrumentEntity>> GetBySymbolsAsync(
        IReadOnlyCollection<string> symbols, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(symbols);

        return dbContext.Instruments
            .Where(instrument => symbols.Contains(instrument.Symbol))
            .ToListAsync(cancellationToken);
    }

    public Task<int> ClearVolume24hExceptAsync(
        IReadOnlyCollection<string> symbols, CancellationToken cancellationToken)
    {
        return dbContext.Instruments.Where(instrument => !symbols.Contains(instrument.Symbol)).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(instrument => instrument.Volume24h, 0m)
                .SetProperty(instrument => instrument.VolumeLastUpdatedOn, (DateTime?)null),
            cancellationToken);
    }

    public Task BulkUpdateAsync(List<InstrumentEntity> instruments, CancellationToken cancellationToken)
    {
        dbContext.Instruments.UpdateRange(instruments);
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
