using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;

namespace PaymentService.Application.Features.CreditCards.Queries.GetDefaultCard;

public class GetDefaultCardQueryHandler : IRequestHandler<GetDefaultCardQuery, CreditCardDto?>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDefaultCardQueryHandler> _logger;

    public GetDefaultCardQueryHandler(
        ICreditCardRepository creditCardRepository,
        IMapper mapper,
        ILogger<GetDefaultCardQueryHandler> logger)
    {
        _creditCardRepository = creditCardRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreditCardDto?> Handle(GetDefaultCardQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 Getting default credit card for user: {UserId}", request.UserId);

        var creditCard = await _creditCardRepository.GetDefaultCardByUserIdAsync(request.UserId);

        if (creditCard == null)
        {
            _logger.LogWarning("⚠️ No default credit card found for user: {UserId}", request.UserId);
            return null;
        }

        _logger.LogInformation("✅ Default credit card found for user: {UserId}", request.UserId);

        return _mapper.Map<CreditCardDto>(creditCard);
    }
}
