using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.RemoveItemFromBasket;

public class RemoveItemFromBasketCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
}
