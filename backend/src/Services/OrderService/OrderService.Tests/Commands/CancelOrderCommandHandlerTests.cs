using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OrderService.Application.Features.Orders.Commands.CancelOrder;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using TechnologyStore.Shared.Events.Orders;
using DomainOrderStatus = OrderService.Domain.Enums.OrderStatus;

namespace OrderService.Tests.Commands;

public class CancelOrderCommandHandlerTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CancelOrderCommandHandler> _logger;
    private readonly CancelOrderCommandHandler _handler;

    public CancelOrderCommandHandlerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _logger = Substitute.For<ILogger<CancelOrderCommandHandler>>();

        _handler = new CancelOrderCommandHandler(
            _orderRepository,
            _publishEndpoint,
            _logger);
    }

    [Fact]
    public async Task Handle_ShouldCancelOrder_WhenOrderCanBeCancelled()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(orderId);
        var command = new CancelOrderCommand
        {
            OrderId = orderId,
            CancellationReason = "Customer changed mind"
        };

        _orderRepository.GetByIdAsync(orderId).Returns(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        order.Status.Should().Be(DomainOrderStatus.Cancelled);
        order.CancellationReason.Should().Be(command.CancellationReason);
        order.CancelledDate.Should().NotBeNull();

        await _orderRepository.Received(1).UpdateAsync(order);
        await _publishEndpoint.Received(1).Publish(
            Arg.Any<OrderCancelledEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenOrderNotFound()
    {
        // Arrange
        var command = new CancelOrderCommand
        {
            OrderId = Guid.NewGuid(),
            CancellationReason = "Test reason"
        };

        _orderRepository.GetByIdAsync(command.OrderId).Returns((Order?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        await _orderRepository.DidNotReceive().UpdateAsync(Arg.Any<Order>());
        await _publishEndpoint.DidNotReceive().Publish(
            Arg.Any<OrderCancelledEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenOrderCannotBeCancelled()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(orderId);
        order.UpdateStatus(DomainOrderStatus.Delivered);

        var command = new CancelOrderCommand
        {
            OrderId = orderId,
            CancellationReason = "Test reason"
        };

        _orderRepository.GetByIdAsync(orderId).Returns(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        await _orderRepository.DidNotReceive().UpdateAsync(Arg.Any<Order>());
        await _publishEndpoint.DidNotReceive().Publish(
            Arg.Any<OrderCancelledEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(DomainOrderStatus.Pending)]
    [InlineData(DomainOrderStatus.PaymentReceived)]
    [InlineData(DomainOrderStatus.Processing)]
    public async Task Handle_ShouldAllowCancellation_ForCancellableStatuses(DomainOrderStatus status)
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(orderId);
        order.UpdateStatus(status);

        var command = new CancelOrderCommand
        {
            OrderId = orderId,
            CancellationReason = "Test cancellation"
        };

        _orderRepository.GetByIdAsync(orderId).Returns(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        order.Status.Should().Be(DomainOrderStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_ShouldPublishOrderCancelledEvent_WithCorrectData()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(orderId);
        var cancellationReason = "Out of stock";

        var command = new CancelOrderCommand
        {
            OrderId = orderId,
            CancellationReason = cancellationReason
        };

        _orderRepository.GetByIdAsync(orderId).Returns(order);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _publishEndpoint.Received(1).Publish(
            Arg.Is<OrderCancelledEvent>(e =>
                e.OrderId == orderId &&
                e.UserId == order.UserId &&
                e.Reason == cancellationReason),
            Arg.Any<CancellationToken>());
    }

    private static Order CreateTestOrder(Guid orderId)
    {
        var address = new Address("Test Street", "Test City", "Test State", "12345", "Test Country");
        var order = new Order("test-user", address)
        {
            Id = orderId
        };
        return order;
    }
}
