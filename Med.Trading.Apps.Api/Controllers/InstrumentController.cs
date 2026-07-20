using Med.Trading.Apps.Api.Contracts;
using Med.Trading.Core.Domain.Entities;
using Med.Trading.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Med.Trading;

[ApiController, Route("[controller]")]
public class InstrumentController(IInstrumentService instrumentService) : ControllerBase
{
    /// <summary>
    /// Top instruments.
    /// </summary>
    [HttpGet("top")]
    [ProducesResponseType(typeof(IReadOnlyList<InstrumentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopInstrumentsAsync(CancellationToken cancellationToken)
    {
        var instruments = await instrumentService.GetTopInstrumentsByVolumeAsync(cancellationToken: cancellationToken);
        return Ok(instruments.Select(ToResponse).ToList());
    }
    
    /// <summary>
    /// Most popular trading this week.
    /// </summary>
    [HttpGet("weekly-popular")]
    [ProducesResponseType(typeof(IReadOnlyList<InstrumentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWeeklyTrendingAsync(CancellationToken cancellationToken)
    {
        var instruments = await instrumentService.GetWeeklyTrendingInstrumentsAsync(cancellationToken: cancellationToken);
        return Ok(instruments.Select(ToResponse).ToList());
    }

    /// <summary>
    /// Gets the current instrument details for a symbol.
    /// </summary>
    /// <param name="symbol">The unique instrument symbol.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>The instrument response when found.</returns>
    [HttpGet("{symbol}")]
    [ProducesResponseType(typeof(InstrumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInstrumentBySymbolAsync(
        string symbol, CancellationToken cancellationToken)
    {
        var instrument = await instrumentService.GetInstrumentBySymbolAsync(symbol, cancellationToken);
        return instrument is null ? NotFound() : Ok(ToResponse(instrument));
    }

    private static InstrumentResponse ToResponse(InstrumentEntity instrument) => new()
    {
        Id = instrument.Id,
        Symbol = instrument.Symbol,
        CurrencyCode = instrument.CurrencyCode,
        Name = instrument.Name,
        DefaultProvider = instrument.DefaultProvider,
        Price = instrument.Price,
        Volume24h = instrument.Volume24h,
        IsActive = instrument.IsActive,
        UpdatedOn = instrument.UpdatedOn
    };
}
