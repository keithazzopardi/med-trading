using Med.Trading.Core.Domain.Entities;

namespace Med.Trading.Core.Domain;

public interface IInstrumentRepository
{
    Task<List<InstrumentEntity>> GetTrendingInsturmentsAsync(int count, int days, CancellationToken cancellationToken);
    Task<List<InstrumentEntity>> GetTopInstrumentsByVolumeAsync(int count, CancellationToken cancellationToken);
    Task<InstrumentEntity?> GetBySymbolAsync(string symbol, CancellationToken cancellationToken);
    Task<List<InstrumentEntity>> GetBySymbolsAsync(
        IReadOnlyCollection<string> symbols, CancellationToken cancellationToken);
    Task<int> ClearVolume24hExceptAsync(
        IReadOnlyCollection<string> symbols, CancellationToken cancellationToken);

    Task BulkUpdateAsync(List<InstrumentEntity> instruments, CancellationToken cancellationToken);
}
