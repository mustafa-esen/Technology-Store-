using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;

namespace PaymentService.Application.Features.CreditCards.Queries.GetCreditCardById;

public class GetCreditCardByIdQueryHandler : IRequestHandler<GetCreditCardByIdQuery, CreditCardDto?>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCreditCardByIdQueryHandler> _logger;

    public GetCreditCardByIdQueryHandler(
        ICreditCardRepository creditCardRepository,
        IMapper mapper,
        ILogger<GetCreditCardByIdQueryHandler> logger)
    {
        _creditCardRepository = creditCardRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreditCardDto?> Handle(GetCreditCardByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 Getting credit card: {CardId} for user: {UserId}",
            request.Id, request.UserId);

        var creditCard = await _creditCardRepository.GetByIdAsync(request.Id);

        if (creditCard == null)
        {
            _logger.LogWarning("⚠️ Credit card not found: {CardId}", request.Id);
            return null;
        }

        if (creditCard.UserId != request.UserId)
        {
            _logger.LogWarning("⚠️ User {UserId} tried to access card owned by {OwnerId}",
                request.UserId, creditCard.UserId);
            return null;
        }

        _logger.LogInformation("✅ Credit card found: {CardId}", request.Id);

        return _mapper.Map<CreditCardDto>(creditCard);
    }
}
