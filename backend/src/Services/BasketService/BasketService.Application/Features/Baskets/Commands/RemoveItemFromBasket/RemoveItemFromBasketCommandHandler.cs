using BasketService.Application.Interfaces;
using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.RemoveItemFromBasket;

public class RemoveItemFromBasketCommandHandler : IRequestHandler<RemoveItemFromBasketCommand, bool>
{
    private readonly IBasketRepository _basketRepository;

    public RemoveItemFromBasketCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<bool> Handle(RemoveItemFromBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.UserId);

        if (basket == null)
        {
            return false;
        }

        basket.RemoveItem(request.ProductId);

        await _basketRepository.SaveBasketAsync(basket);

        return true;
    }
}
