using FluentAssertions;
using Med.Trading.Core.Domain;
using Med.Trading.Core.Domain.Entities;
using Med.Trading.Core.Services.Abstractions;
using Med.Trading.Core.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Med.Trading.Tests.UnitTests;

public sealed class TradeOrderServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_NewHolding_PersistsHoldingAndPublishesEvent()
    {
        // Arrange
        var instrumentRepository = CreateInstrumentRepository();
        var orderRepository = new Mock<ITradingOrderRepository>();
        orderRepository
            .Setup(repository => repository.GetHoldingAsync(7, "MED", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserHoldingEntity?)null);
        orderRepository
            .Setup(repository => repository.CreateHoldingAsync(It.IsAny<UserHoldingEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var eventPublisher = new Mock<IEventPublisher>();
        var service = CreateService(orderRepository, instrumentRepository, eventPublisher);

        // Act
        var result = await service.CreateOrderAsync(CreateRequest(), 7);

        // Assert
        result.IsSuccess.Should().BeTrue();
        orderRepository.Verify(
            repository => repository.CreateHoldingAsync(
                It.Is<UserHoldingEntity>(holding =>
                    holding.UserId == 7 &&
                    holding.TradeOrders.Single().OrderType == TraderOrderType.StopLimit),
                It.IsAny<CancellationToken>()),
            Times.Once);
        eventPublisher.Verify(
            publisher => publisher.PublishAsync(
                It.Is<TradeOrderEntity>(order => order.UserId == 7 && order.InstrumentSymbol == "MED"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_ExistingHolding_AddsOnlyTheNewOrder()
    {
        // Arrange
        var holding = new UserHoldingEntity { Id = 10, UserId = 7, InstrumentSymbol = "MED" };
        var instrumentRepository = CreateInstrumentRepository();
        var orderRepository = new Mock<ITradingOrderRepository>();
        orderRepository
            .Setup(repository => repository.GetHoldingAsync(7, "MED", It.IsAny<CancellationToken>()))
            .ReturnsAsync(holding);
        orderRepository
            .Setup(repository => repository.AddOrderAsync(It.IsAny<TradeOrderEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var eventPublisher = new Mock<IEventPublisher>();
        var service = CreateService(orderRepository, instrumentRepository, eventPublisher);

        // Act
        var result = await service.CreateOrderAsync(CreateRequest(), 7);

        // Assert
        result.IsSuccess.Should().BeTrue();
        holding.TradeOrders.Should().ContainSingle();
        orderRepository.Verify(
            repository => repository.AddOrderAsync(holding.TradeOrders.Single(), It.IsAny<CancellationToken>()),
            Times.Once);
        orderRepository.Verify(
            repository => repository.CreateHoldingAsync(It.IsAny<UserHoldingEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetPendingOrdersAsync_UserId_DelegatesToRepository()
    {
        // Arrange
        var expectedOrders = new List<TradeOrderEntity> { new() { UserId = 7, Status = OrderStatus.Pending } };
        var instrumentRepository = CreateInstrumentRepository();
        var orderRepository = new Mock<ITradingOrderRepository>();
        orderRepository
            .Setup(repository => repository.GetPendingOrdersAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrders);

        var service = CreateService(orderRepository, instrumentRepository, new Mock<IEventPublisher>());

        // Act
        var result = await service.GetPendingOrdersAsync(7);

        // Assert
        result.Should().BeSameAs(expectedOrders);
    }

    private static TradeOrderService CreateService(
        Mock<ITradingOrderRepository> orderRepository,
        Mock<IInstrumentRepository> instrumentRepository,
        Mock<IEventPublisher> eventPublisher) =>
        new(
            orderRepository.Object,
            instrumentRepository.Object,
            eventPublisher.Object,
            new Mock<ILogger<TradeOrderService>>().Object);

    private static Mock<IInstrumentRepository> CreateInstrumentRepository()
    {
        var repository = new Mock<IInstrumentRepository>();
        repository
            .Setup(value => value.GetBySymbolAsync("MED", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentEntity { Symbol = "MED", IsActive = true, Price = 1.25m });
        return repository;
    }

    private static CreateTradeOrderRequest CreateRequest() => new(
        "MED",
        2,
        1.25m,
        OrderSide.Buy,
        TraderOrderType.StopLimit);
}
