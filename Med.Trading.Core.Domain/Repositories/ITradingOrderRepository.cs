using Med.Trading.Core.Domain.Entities;

namespace Med.Trading.Core.Domain;

public interface ITradingOrderRepository
{
    Task<IReadOnlyList<TradeOrderEntity>> GetPendingOrdersAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<UserHoldingEntity?> GetHoldingAsync(
        long userId,
        string instrumentSymbol,
        CancellationToken cancellationToken = default);

    Task CreateHoldingAsync(
        UserHoldingEntity holding,
        CancellationToken cancellationToken = default);

    Task AddOrderAsync(
        TradeOrderEntity order,
        CancellationToken cancellationToken = default);

    Task<Dictionary<string, decimal>> GetFilledVolumeByInstrumentAsync(
        DateTime windowStartUtc,
        int take,
        CancellationToken cancellationToken = default);
}
