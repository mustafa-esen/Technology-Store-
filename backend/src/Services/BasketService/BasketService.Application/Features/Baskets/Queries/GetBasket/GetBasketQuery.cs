using BasketService.Application.DTOs;
using MediatR;

namespace BasketService.Application.Features.Baskets.Queries.GetBasket;

public class GetBasketQuery : IRequest<BasketDto?>
{
    public string UserId { get; set; } = string.Empty;
}
