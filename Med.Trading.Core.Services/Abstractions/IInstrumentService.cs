using Med.Trading.Core.Domain.Entities;

namespace Med.Trading.Core.Services.Abstractions;

public interface IInstrumentService
{
    Task<IList<InstrumentEntity>> GetTopInstrumentsByVolumeAsync(int count = 10,
        CancellationToken cancellationToken = default);

    Task<InstrumentEntity?> GetInstrumentBySymbolAsync(string symbol,
        CancellationToken cancellationToken = default);

    Task<IList<InstrumentEntity>> GetWeeklyTrendingInstrumentsAsync(int count = 10, CancellationToken cancellationToken = default);

    Task RefreshVolume24hAsync(CancellationToken cancellationToken = default);
}
