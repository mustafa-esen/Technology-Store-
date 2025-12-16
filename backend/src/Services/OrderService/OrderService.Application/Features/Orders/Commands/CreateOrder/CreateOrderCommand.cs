using MediatR;
using OrderService.Application.DTOs;
using TechnologyStore.Shared.Events.Baskets;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<OrderDto>
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
    public CreateAddressDto ShippingAddress { get; set; } = null!;
    public string? Notes { get; set; }

    // BasketCheckoutEvent'ten mapping için factory method
    public static CreateOrderCommand FromBasketCheckoutEvent(IBasketCheckoutEvent basketEvent)
    {
        return new CreateOrderCommand
        {
            UserId = basketEvent.UserId,
            UserName = basketEvent.UserName,
            TotalPrice = basketEvent.TotalPrice,
            ShippingAddress = new CreateAddressDto
            {
                Street = basketEvent.ShippingAddress.Street,
                City = basketEvent.ShippingAddress.City,
                State = basketEvent.ShippingAddress.State,
                ZipCode = basketEvent.ShippingAddress.ZipCode,
                Country = basketEvent.ShippingAddress.Country
            },
            Items = basketEvent.Items.Select(item => new CreateOrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}

