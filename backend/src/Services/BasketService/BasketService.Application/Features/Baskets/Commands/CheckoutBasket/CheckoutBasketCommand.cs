using MediatR;
using TechnologyStore.Shared.Events.Baskets;

namespace BasketService.Application.Features.Baskets.Commands.CheckoutBasket;

public class CheckoutBasketCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public BasketCheckoutAddressDto ShippingAddress { get; set; } = null!;
}
