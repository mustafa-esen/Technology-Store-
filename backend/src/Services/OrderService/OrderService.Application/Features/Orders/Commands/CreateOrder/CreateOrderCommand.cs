using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<OrderDto>
{
    public string UserId { get; set; } = string.Empty;
    public List<CreateOrderItemDto> Items { get; set; } = new();
    public CreateAddressDto ShippingAddress { get; set; } = null!;
    public string? Notes { get; set; }
}
