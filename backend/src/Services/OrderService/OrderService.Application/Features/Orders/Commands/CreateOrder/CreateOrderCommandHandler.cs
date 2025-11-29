using AutoMapper;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using TechnologyStore.Shared.Events.Orders;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IMapper mapper,
        IPublishEndpoint publishEndpoint,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new order for user: {UserId}", request.UserId);

        // Create shipping address
        var shippingAddress = new Address(
            request.ShippingAddress.Street,
            request.ShippingAddress.City,
            request.ShippingAddress.State,
            request.ShippingAddress.ZipCode,
            request.ShippingAddress.Country
        );

        // Create order
        var order = new Order(request.UserId, shippingAddress)
        {
            Notes = request.Notes
        };

        // Add items
        foreach (var itemDto in request.Items)
        {
            var orderItem = new OrderItem(
                itemDto.ProductId,
                itemDto.ProductName,
                itemDto.Price,
                itemDto.Quantity
            );
            order.AddItem(orderItem);
        }

        // Save to database
        var createdOrder = await _orderRepository.CreateAsync(order);

        _logger.LogInformation("Order created successfully. OrderId: {OrderId}, TotalAmount: {TotalAmount}",
            createdOrder.Id, createdOrder.TotalAmount);

        // Publish OrderCreatedEvent
        var orderCreatedEvent = _mapper.Map<OrderCreatedEvent>(createdOrder);
        await _publishEndpoint.Publish(orderCreatedEvent, cancellationToken);

        _logger.LogInformation("OrderCreatedEvent published for OrderId: {OrderId}", createdOrder.Id);

        return _mapper.Map<OrderDto>(createdOrder);
    }
}
