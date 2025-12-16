using AutoMapper;
using FluentAssertions;
using NSubstitute;
using OrderService.Application.DTOs;
using OrderService.Application.Features.Orders.Queries.GetOrder;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.ValueObjects;

namespace OrderService.Tests.Queries;

public class GetOrderQueryHandlerTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly GetOrderQueryHandler _handler;

    public GetOrderQueryHandlerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _mapper = Substitute.For<IMapper>();

        _handler = new GetOrderQueryHandler(_orderRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrderDto_WhenOrderExists()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(orderId);
        var orderDto = new OrderDto
        {
            Id = orderId,
            UserId = order.UserId,
            Status = order.Status,
            TotalAmount = order.TotalAmount
        };

        var query = new GetOrderQuery { OrderId = orderId };

        _orderRepository.GetByIdWithItemsAsync(orderId).Returns(order);
        _mapper.Map<OrderDto>(order).Returns(orderDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        result.UserId.Should().Be(order.UserId);

        await _orderRepository.Received(1).GetByIdWithItemsAsync(orderId);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenOrderNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderQuery { OrderId = orderId };

        _orderRepository.GetByIdWithItemsAsync(orderId).Returns((Order?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        await _orderRepository.Received(1).GetByIdWithItemsAsync(orderId);
        _mapper.DidNotReceive().Map<OrderDto>(Arg.Any<Order>());
    }

    [Fact]
    public async Task Handle_ShouldMapOrderCorrectly()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(orderId);
        order.AddItem(new OrderItem(Guid.NewGuid(), "Test Product", 100m, 2));

        var orderDto = new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            TotalAmount = 200m
        };

        var query = new GetOrderQuery { OrderId = orderId };

        _orderRepository.GetByIdWithItemsAsync(orderId).Returns(order);
        _mapper.Map<OrderDto>(order).Returns(orderDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.TotalAmount.Should().Be(200m);
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
