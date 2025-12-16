using BasketService.Application.Interfaces;
using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.UpdateItemQuantity;

public class UpdateItemQuantityCommandHandler : IRequestHandler<UpdateItemQuantityCommand, bool>
{
    private readonly IBasketRepository _basketRepository;

    public UpdateItemQuantityCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<bool> Handle(UpdateItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.UserId);

        if (basket == null)
        {
            return false;
        }

        basket.UpdateItemQuantity(request.ProductId, request.Quantity);

        await _basketRepository.SaveBasketAsync(basket);

        return true;
    }
}
