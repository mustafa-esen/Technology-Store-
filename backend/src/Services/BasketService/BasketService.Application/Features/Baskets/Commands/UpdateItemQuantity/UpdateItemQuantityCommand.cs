using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.UpdateItemQuantity;

public class UpdateItemQuantityCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
