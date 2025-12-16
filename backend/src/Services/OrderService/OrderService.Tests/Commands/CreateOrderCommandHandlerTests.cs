using AutoMapper;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OrderService.Application.DTOs;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using TechnologyStore.Shared.Events.Orders;
using DomainOrderStatus = OrderService.Domain.Enums.OrderStatus;

namespace OrderService.Tests.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _mapper = Substitute.For<IMapper>();
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _logger = Substitute.For<ILogger<CreateOrderCommandHandler>>();

        _handler = new CreateOrderCommandHandler(
            _orderRepository,
            _mapper,
            _publishEndpoint,
            _logger);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrder_WithValidCommand()
    {
        // Arrange
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var command = new CreateOrderCommand
        {
            UserId = "user-123",
            ShippingAddress = new CreateAddressDto
            {
                Street = "123 Main St",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                Country = "USA"
            },
            Notes = "Please deliver fast",
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = productId1,
                    ProductName = "Product 1",
                    Price = 100m,
                    Quantity = 2
                },
                new()
                {
                    ProductId = productId2,
                    ProductName = "Product 2",
                    Price = 50m,
                    Quantity = 1
                }
            }
        };

        var createdOrderId = Guid.NewGuid();
        var createdOrder = new Order(command.UserId, new Address(
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.State,
            command.ShippingAddress.ZipCode,
            command.ShippingAddress.Country))
        {
            Id = createdOrderId,
            Notes = command.Notes
        };

        var orderDto = new OrderDto
        {
            Id = createdOrderId,
            UserId = command.UserId,
            Status = DomainOrderStatus.Pending,
            TotalAmount = 250m
        };

        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = createdOrderId,
            UserId = command.UserId
        };

        _orderRepository.CreateAsync(Arg.Any<Order>()).Returns(createdOrder);
        _mapper.Map<OrderDto>(Arg.Any<Order>()).Returns(orderDto);
        _mapper.Map<OrderCreatedEvent>(Arg.Any<Order>()).Returns(orderCreatedEvent);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(createdOrderId);
        result.UserId.Should().Be(command.UserId);
        result.Status.Should().Be(DomainOrderStatus.Pending);

        await _orderRepository.Received(1).CreateAsync(Arg.Is<Order>(o =>
            o.UserId == command.UserId &&
            o.ShippingAddress.Street == command.ShippingAddress.Street &&
            o.Notes == command.Notes));

        await _publishEndpoint.Received(1).Publish(
            Arg.Any<OrderCreatedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldAddAllItems_ToOrder()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            UserId = "user-456",
            ShippingAddress = new CreateAddressDto
            {
                Street = "456 Oak Ave",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90001",
                Country = "USA"
            },
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Laptop",
                    Price = 1500m,
                    Quantity = 1
                },
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Mouse",
                    Price = 25m,
                    Quantity = 2
                },
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Keyboard",
                    Price = 75m,
                    Quantity = 1
                }
            }
        };

        var createdOrder = new Order(command.UserId, new Address(
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.State,
            command.ShippingAddress.ZipCode,
            command.ShippingAddress.Country))
        {
            Id = Guid.NewGuid()
        };

        _orderRepository.CreateAsync(Arg.Any<Order>()).Returns(createdOrder);
        _mapper.Map<OrderDto>(Arg.Any<Order>()).Returns(new OrderDto());
        _mapper.Map<OrderCreatedEvent>(Arg.Any<Order>()).Returns(new OrderCreatedEvent());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _orderRepository.Received(1).CreateAsync(Arg.Is<Order>(o =>
            o.Items.Count == 3 &&
            o.Items.Any(i => i.ProductName == "Laptop") &&
            o.Items.Any(i => i.ProductName == "Mouse") &&
            o.Items.Any(i => i.ProductName == "Keyboard")));
    }

    [Fact]
    public async Task Handle_ShouldPublishOrderCreatedEvent_WithCorrectData()
    {
        // Arrange
        var userId = "user-789";
        var orderId = Guid.NewGuid();

        var command = new CreateOrderCommand
        {
            UserId = userId,
            ShippingAddress = new CreateAddressDto
            {
                Street = "789 Elm St",
                City = "Chicago",
                State = "IL",
                ZipCode = "60601",
                Country = "USA"
            },
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Monitor",
                    Price = 300m,
                    Quantity = 1
                }
            }
        };

        var createdOrder = new Order(command.UserId, new Address(
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.State,
            command.ShippingAddress.ZipCode,
            command.ShippingAddress.Country))
        {
            Id = orderId
        };

        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = orderId,
            UserId = userId
        };

        _orderRepository.CreateAsync(Arg.Any<Order>()).Returns(createdOrder);
        _mapper.Map<OrderDto>(Arg.Any<Order>()).Returns(new OrderDto());
        _mapper.Map<OrderCreatedEvent>(Arg.Any<Order>()).Returns(orderCreatedEvent);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _publishEndpoint.Received(1).Publish(
            Arg.Is<OrderCreatedEvent>(e =>
                e.OrderId == orderId &&
                e.UserId == userId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldMapShippingAddress_Correctly()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            UserId = "user-999",
            ShippingAddress = new CreateAddressDto
            {
                Street = "999 Pine Rd",
                City = "Houston",
                State = "TX",
                ZipCode = "77001",
                Country = "USA"
            },
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    Price = 50m,
                    Quantity = 1
                }
            }
        };

        var createdOrder = new Order(command.UserId, new Address(
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.State,
            command.ShippingAddress.ZipCode,
            command.ShippingAddress.Country))
        {
            Id = Guid.NewGuid()
        };

        _orderRepository.CreateAsync(Arg.Any<Order>()).Returns(createdOrder);
        _mapper.Map<OrderDto>(Arg.Any<Order>()).Returns(new OrderDto());
        _mapper.Map<OrderCreatedEvent>(Arg.Any<Order>()).Returns(new OrderCreatedEvent());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _orderRepository.Received(1).CreateAsync(Arg.Is<Order>(o =>
            o.ShippingAddress.Street == "999 Pine Rd" &&
            o.ShippingAddress.City == "Houston" &&
            o.ShippingAddress.State == "TX" &&
            o.ShippingAddress.ZipCode == "77001" &&
            o.ShippingAddress.Country == "USA"));
    }

    [Fact]
    public async Task Handle_ShouldSetOrderStatus_ToPending()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            UserId = "user-111",
            ShippingAddress = new CreateAddressDto
            {
                Street = "111 Maple Dr",
                City = "Phoenix",
                State = "AZ",
                ZipCode = "85001",
                Country = "USA"
            },
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Tablet",
                    Price = 400m,
                    Quantity = 1
                }
            }
        };

        var createdOrder = new Order(command.UserId, new Address(
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.State,
            command.ShippingAddress.ZipCode,
            command.ShippingAddress.Country))
        {
            Id = Guid.NewGuid()
        };

        _orderRepository.CreateAsync(Arg.Any<Order>()).Returns(createdOrder);
        _mapper.Map<OrderDto>(Arg.Any<Order>()).Returns(new OrderDto());
        _mapper.Map<OrderCreatedEvent>(Arg.Any<Order>()).Returns(new OrderCreatedEvent());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _orderRepository.Received(1).CreateAsync(Arg.Is<Order>(o =>
            o.Status == DomainOrderStatus.Pending));
    }

    [Fact]
    public async Task Handle_ShouldCallMapper_ForOrderDto()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            UserId = "user-222",
            ShippingAddress = new CreateAddressDto
            {
                Street = "222 Cedar Ln",
                City = "Philadelphia",
                State = "PA",
                ZipCode = "19019",
                Country = "USA"
            },
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Headphones",
                    Price = 150m,
                    Quantity = 1
                }
            }
        };

        var createdOrder = new Order(command.UserId, new Address(
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.State,
            command.ShippingAddress.ZipCode,
            command.ShippingAddress.Country))
        {
            Id = Guid.NewGuid()
        };

        _orderRepository.CreateAsync(Arg.Any<Order>()).Returns(createdOrder);
        _mapper.Map<OrderDto>(Arg.Any<Order>()).Returns(new OrderDto());
        _mapper.Map<OrderCreatedEvent>(Arg.Any<Order>()).Returns(new OrderCreatedEvent());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map<OrderDto>(Arg.Is<Order>(o => o.UserId == command.UserId));
    }

    [Fact]
    public async Task Handle_ShouldCallMapper_ForOrderCreatedEvent()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            UserId = "user-333",
            ShippingAddress = new CreateAddressDto
            {
                Street = "333 Birch Ave",
                City = "San Antonio",
                State = "TX",
                ZipCode = "78201",
                Country = "USA"
            },
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Camera",
                    Price = 800m,
                    Quantity = 1
                }
            }
        };

        var createdOrder = new Order(command.UserId, new Address(
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.State,
            command.ShippingAddress.ZipCode,
            command.ShippingAddress.Country))
        {
            Id = Guid.NewGuid()
        };

        _orderRepository.CreateAsync(Arg.Any<Order>()).Returns(createdOrder);
        _mapper.Map<OrderDto>(Arg.Any<Order>()).Returns(new OrderDto());
        _mapper.Map<OrderCreatedEvent>(Arg.Any<Order>()).Returns(new OrderCreatedEvent());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map<OrderCreatedEvent>(Arg.Is<Order>(o => o.UserId == command.UserId));
    }

    [Fact]
    public async Task Handle_ShouldSetNotes_WhenProvided()
    {
        // Arrange
        var notes = "Handle with care - fragile items";
        var command = new CreateOrderCommand
        {
            UserId = "user-444",
            ShippingAddress = new CreateAddressDto
            {
                Street = "444 Willow St",
                City = "San Diego",
                State = "CA",
                ZipCode = "92101",
                Country = "USA"
            },
            Notes = notes,
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Glass Vase",
                    Price = 45m,
                    Quantity = 2
                }
            }
        };

        var createdOrder = new Order(command.UserId, new Address(
            command.ShippingAddress.Street,
            command.ShippingAddress.City,
            command.ShippingAddress.State,
            command.ShippingAddress.ZipCode,
            command.ShippingAddress.Country))
        {
            Id = Guid.NewGuid(),
            Notes = notes
        };

        _orderRepository.CreateAsync(Arg.Any<Order>()).Returns(createdOrder);
        _mapper.Map<OrderDto>(Arg.Any<Order>()).Returns(new OrderDto());
        _mapper.Map<OrderCreatedEvent>(Arg.Any<Order>()).Returns(new OrderCreatedEvent());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _orderRepository.Received(1).CreateAsync(Arg.Is<Order>(o =>
            o.Notes == notes));
    }
}
