using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.AddItemToBasket;

public class AddItemToBasketCommandHandler : IRequestHandler<AddItemToBasketCommand, bool>
{
    private readonly IBasketRepository _basketRepository;

    public AddItemToBasketCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<bool> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.UserId);

        if (basket == null)
        {
            basket = new Basket(request.UserId);
        }

        var basketItem = new BasketItem(
            request.ProductId,
            request.ProductName,
            request.Price,
            request.Quantity
        );

        basket.AddItem(basketItem);

        await _basketRepository.SaveBasketAsync(basket);

        return true;
    }
}
