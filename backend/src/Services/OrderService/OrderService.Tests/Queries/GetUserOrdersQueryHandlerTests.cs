using AutoMapper;
using FluentAssertions;
using NSubstitute;
using OrderService.Application.DTOs;
using OrderService.Application.Features.Orders.Queries.GetUserOrders;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;

namespace OrderService.Tests.Queries;

public class GetUserOrdersQueryHandlerTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly GetUserOrdersQueryHandler _handler;

    public GetUserOrdersQueryHandlerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _mapper = Substitute.For<IMapper>();

        _handler = new GetUserOrdersQueryHandler(_orderRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserOrders_WhenOrdersExist()
    {
        // Arrange
        var userId = "user-123";
        var orders = new List<Order>
        {
            CreateTestOrder(Guid.NewGuid(), userId),
            CreateTestOrder(Guid.NewGuid(), userId)
        };

        var orderDtos = new List<OrderDto>
        {
            new() { Id = orders[0].Id, UserId = userId },
            new() { Id = orders[1].Id, UserId = userId }
        };

        var query = new GetUserOrdersQuery { UserId = userId };

        _orderRepository.GetUserOrdersAsync(userId).Returns(orders);
        _mapper.Map<List<OrderDto>>(orders).Returns(orderDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(o => o.UserId == userId).Should().BeTrue();

        await _orderRepository.Received(1).GetUserOrdersAsync(userId);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoOrdersFound()
    {
        // Arrange
        var userId = "user-123";
        var orders = new List<Order>();
        var orderDtos = new List<OrderDto>();

        var query = new GetUserOrdersQuery { UserId = userId };

        _orderRepository.GetUserOrdersAsync(userId).Returns(orders);
        _mapper.Map<List<OrderDto>>(orders).Returns(orderDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        await _orderRepository.Received(1).GetUserOrdersAsync(userId);
    }

    [Fact]
    public async Task Handle_ShouldMapAllOrders()
    {
        // Arrange
        var userId = "user-123";
        var orders = new List<Order>
        {
            CreateTestOrder(Guid.NewGuid(), userId),
            CreateTestOrder(Guid.NewGuid(), userId),
            CreateTestOrder(Guid.NewGuid(), userId)
        };

        var orderDtos = orders.Select(o => new OrderDto { Id = o.Id, UserId = userId }).ToList();

        var query = new GetUserOrdersQuery { UserId = userId };

        _orderRepository.GetUserOrdersAsync(userId).Returns(orders);
        _mapper.Map<List<OrderDto>>(orders).Returns(orderDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        _mapper.Received(1).Map<List<OrderDto>>(orders);
    }

    private static Order CreateTestOrder(Guid orderId, string userId)
    {
        var address = new Address("Test Street", "Test City", "Test State", "12345", "Test Country");
        var order = new Order(userId, address)
        {
            Id = orderId
        };
        return order;
    }
}
