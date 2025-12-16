using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;

namespace PaymentService.Application.Features.CreditCards.Queries.GetUserCreditCards;

public class GetUserCreditCardsQueryHandler : IRequestHandler<GetUserCreditCardsQuery, List<CreditCardDto>>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserCreditCardsQueryHandler> _logger;

    public GetUserCreditCardsQueryHandler(
        ICreditCardRepository creditCardRepository,
        IMapper mapper,
        ILogger<GetUserCreditCardsQueryHandler> logger)
    {
        _creditCardRepository = creditCardRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<CreditCardDto>> Handle(GetUserCreditCardsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 Getting credit cards for user: {UserId}", request.UserId);

        var creditCards = await _creditCardRepository.GetByUserIdAsync(request.UserId);

        _logger.LogInformation("✅ Found {Count} credit card(s) for user: {UserId}",
            creditCards.Count, request.UserId);

        return _mapper.Map<List<CreditCardDto>>(creditCards);
    }
}
