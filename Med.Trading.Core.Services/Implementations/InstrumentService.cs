using Med.Trading.Core.Domain;
using Med.Trading.Core.Domain.Entities;
using Med.Trading.Core.Services.Abstractions;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Med.Trading.Core.Services.Implementations;

internal class InstrumentService(
    IInstrumentRepository instrumentRepository,
    ITradingOrderRepository tradingOrderRepository,
    IMemoryCache memoryCache,
    IEnumerable<IDistributedCache> distributedCaches,
    TimeProvider timeProvider,
    ILogger<InstrumentService> logger) : IInstrumentService
{
    private static readonly TimeSpan LocalCacheLifetime = TimeSpan.FromSeconds(10);
    private readonly IDistributedCache? distributedCache = distributedCaches.FirstOrDefault();

    public async Task<IList<InstrumentEntity>> GetTopInstrumentsByVolumeAsync(
        int count = 10, CancellationToken cancellationToken = default)
    {
        return await GetFromCacheOrDatabaseAsync(
            $"instruments:top-volume:{count}",
            token => instrumentRepository.GetTopInstrumentsByVolumeAsync(count, token),
            cancellationToken);
    }

    public async Task<InstrumentEntity?> GetInstrumentBySymbolAsync(
        string symbol, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        cancellationToken.ThrowIfCancellationRequested();

        // Bypass caching so this lookup always observes current database state.
        return await instrumentRepository.GetBySymbolAsync(symbol, cancellationToken);
    }

    public async Task<IList<InstrumentEntity>> GetWeeklyTrendingInstrumentsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await GetFromCacheOrDatabaseAsync(
            $"instruments:top-weekly:{count}",
            token => instrumentRepository.GetTrendingInsturmentsAsync(count, 7, token),
            cancellationToken);
    }

    public async Task RefreshVolume24hAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.LogInformation("Starting instruments volume 24h refresh.");

            var calculatedAtUtc = timeProvider.GetUtcNow().UtcDateTime;
            var windowStartUtc = calculatedAtUtc.AddHours(-24);
            const int maxInstrumentsToUpdate = 50;
            var volumesBySymbol = await tradingOrderRepository
                .GetFilledVolumeByInstrumentAsync(windowStartUtc, maxInstrumentsToUpdate, cancellationToken);

            // Update Volume.
            var instruments = await instrumentRepository.GetBySymbolsAsync(volumesBySymbol.Keys, cancellationToken);
            foreach (var instrument in instruments)
            {
                instrument.UpdateVolume24Hour(volumesBySymbol.GetValueOrDefault(instrument.Symbol), calculatedAtUtc);
            }

            await instrumentRepository.BulkUpdateAsync(instruments, cancellationToken);

            await instrumentRepository.ClearVolume24hExceptAsync(volumesBySymbol.Keys, cancellationToken);

            logger.LogInformation("Completed instruments volume 24h refresh.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error refreshing instruments volume 24h. Exception: {Exception}", ex);
            throw ex;
        }
    }

    private async Task<T> GetFromCacheOrDatabaseAsync<T>(
        string cacheKey,
        Func<CancellationToken, Task<T>> databaseFallback,
        CancellationToken cancellationToken)
        where T : class
    {
        // Get local cache for very short periods.
        if (memoryCache.TryGetValue<T>(cacheKey, out var localValue) && localValue is not null)
            return localValue;

        try
        {
            // Get from redis (if available)
            if (distributedCache is not null)
            {
                var distributedValue = await distributedCache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrWhiteSpace(distributedValue))
                {
                    var cachedValue = JsonSerializer.Deserialize<T>(distributedValue);
                    if (cachedValue is not null)
                    {
                        memoryCache.Set(cacheKey, cachedValue, LocalCacheLifetime);
                        return cachedValue;
                    }
                }
            }
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            // Redis is an optional read-only layer. A failure to reduce always fallback
            // to the database, which remains the source of truth.
        }

        // Always fallback to the database.
        var databaseValue = await databaseFallback(cancellationToken);
        memoryCache.Set(cacheKey, databaseValue, LocalCacheLifetime);
        return databaseValue;
    }
}
