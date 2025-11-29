using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.ClearBasket;

public class ClearBasketCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
}
