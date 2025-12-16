using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.AddItemToBasket;

public class AddItemToBasketCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
