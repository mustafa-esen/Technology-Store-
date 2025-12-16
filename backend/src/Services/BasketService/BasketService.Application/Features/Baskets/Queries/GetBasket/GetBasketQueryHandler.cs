using AutoMapper;
using BasketService.Application.DTOs;
using BasketService.Application.Interfaces;
using MediatR;

namespace BasketService.Application.Features.Baskets.Queries.GetBasket;

public class GetBasketQueryHandler : IRequestHandler<GetBasketQuery, BasketDto?>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;

    public GetBasketQueryHandler(IBasketRepository basketRepository, IMapper mapper)
    {
        _basketRepository = basketRepository;
        _mapper = mapper;
    }

    public async Task<BasketDto?> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.UserId);

        if (basket == null)
        {
            return null;
        }

        return _mapper.Map<BasketDto>(basket);
    }
}
