using Med.Shared;
using Med.Shared.Services.UserContext;
using Med.Trading.Apps.Api.Contracts;
using Med.Trading.Core.Domain.Entities;
using Med.Trading.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Med.Trading;

[ApiController, Route("[controller]")]
public class OrderController(ITradeOrderService tradeOrderService, IUserContext userContext) : ControllerBase
{
    /// <summary>
    /// Gets the authenticated user's pending orders.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>The user's pending order responses.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TradeOrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingOrdersAsync(CancellationToken cancellationToken)
    {
        var orders = await tradeOrderService.GetPendingOrdersAsync(userContext.UserId, cancellationToken);
        return Ok(orders.Select(ToResponse).ToList());
    }

    /// <summary>
    /// Submits a new trade order for the authenticated user.
    /// </summary>
    /// <param name="request">The order details to submit.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>The operation result.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrderAsync(
        [FromBody] CreateTradeOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await tradeOrderService.CreateOrderAsync(request, 5, cancellationToken);
        return Ok(result);
    }

    private static TradeOrderResponse ToResponse(TradeOrderEntity order) => new()
    {
        Id = order.Id,
        InstrumentSymbol = order.InstrumentSymbol,
        Side = order.Side.ToString(),
        OrderType = order.OrderType.ToString(),
        Quantity = order.Quantity,
        Price = order.Price,
        UserId = order.UserId,
        PortfolioId = order.PortfolioId,
        CommissionMoney = order.CommissionMoney,
        Status = order.Status.ToString(),
        CreatedOn = order.CreatedOn,
        FilledOn = order.FilledOn
    };
}
