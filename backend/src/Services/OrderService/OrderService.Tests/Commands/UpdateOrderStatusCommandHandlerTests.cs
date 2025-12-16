using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using TechnologyStore.Shared.Events.Orders;
using DomainOrderStatus = OrderService.Domain.Enums.OrderStatus;

namespace OrderService.Tests.Commands;

public class UpdateOrderStatusCommandHandlerTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;
    private readonly UpdateOrderStatusCommandHandler _handler;

    public UpdateOrderStatusCommandHandlerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _logger = Substitute.For<ILogger<UpdateOrderStatusCommandHandler>>();

        _handler = new UpdateOrderStatusCommandHandler(
            _orderRepository,
            _publishEndpoint,
            _logger);
    }

    [Fact]
    public async Task Handle_ShouldUpdateOrderStatus_WhenOrderExists()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(orderId);
        var command = new UpdateOrderStatusCommand
        {
            OrderId = orderId,
            Status = "Processing"
        };

        _orderRepository.GetByIdAsync(orderId).Returns(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        order.Status.Should().Be(DomainOrderStatus.Processing);

        await _orderRepository.Received(1).UpdateAsync(order);
        await _publishEndpoint.Received(1).Publish(
            Arg.Any<OrderStatusChangedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenOrderNotFound()
    {
        // Arrange
        var command = new UpdateOrderStatusCommand
        {
            OrderId = Guid.NewGuid(),
            Status = "Processing"
        };

        _orderRepository.GetByIdAsync(command.OrderId).Returns((Order?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        await _orderRepository.DidNotReceive().UpdateAsync(Arg.Any<Order>());
        await _publishEndpoint.DidNotReceive().Publish(
            Arg.Any<OrderStatusChangedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldPublishOrderCompletedEvent_WhenStatusIsDelivered()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(orderId);
        var command = new UpdateOrderStatusCommand
        {
            OrderId = orderId,
            Status = "Delivered"
        };

        _orderRepository.GetByIdAsync(orderId).Returns(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        order.Status.Should().Be(DomainOrderStatus.Delivered);
        order.CompletedDate.Should().NotBeNull();

        await _publishEndpoint.Received(1).Publish(
            Arg.Any<OrderStatusChangedEvent>(),
            Arg.Any<CancellationToken>());

        await _publishEndpoint.Received(1).Publish(
            Arg.Any<OrderCompletedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("PaymentReceived")]
    [InlineData("Processing")]
    [InlineData("Shipped")]
    public async Task Handle_ShouldUpdateToVariousStatuses(string status)
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(orderId);
        var command = new UpdateOrderStatusCommand
        {
            OrderId = orderId,
            Status = status
        };

        _orderRepository.GetByIdAsync(orderId).Returns(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        order.Status.ToString().Should().Be(status);
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
