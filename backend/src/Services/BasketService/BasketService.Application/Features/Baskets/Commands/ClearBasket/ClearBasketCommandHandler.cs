using BasketService.Application.Interfaces;
using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.ClearBasket;

public class ClearBasketCommandHandler : IRequestHandler<ClearBasketCommand, bool>
{
    private readonly IBasketRepository _basketRepository;

    public ClearBasketCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<bool> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        return await _basketRepository.DeleteBasketAsync(request.UserId);
    }
}
