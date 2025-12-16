using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;

namespace PaymentService.Application.Features.CreditCards.Commands.UpdateCreditCard;

public class UpdateCreditCardCommandHandler : IRequestHandler<UpdateCreditCardCommand, CreditCardDto>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCreditCardCommandHandler> _logger;

    public UpdateCreditCardCommandHandler(
        ICreditCardRepository creditCardRepository,
        IMapper mapper,
        ILogger<UpdateCreditCardCommandHandler> logger)
    {
        _creditCardRepository = creditCardRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreditCardDto> Handle(UpdateCreditCardCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("✏️ Updating credit card: {CardId} for user: {UserId}",
            request.Id, request.UserId);

        var creditCard = await _creditCardRepository.GetByIdAsync(request.Id);

        if (creditCard == null)
        {
            _logger.LogWarning("⚠️ Credit card not found: {CardId}", request.Id);
            throw new Exception($"Credit card not found: {request.Id}");
        }

        if (creditCard.UserId != request.UserId)
        {
            _logger.LogWarning("⚠️ User {UserId} tried to update card owned by {OwnerId}",
                request.UserId, creditCard.UserId);
            throw new UnauthorizedAccessException("You can only update your own credit cards");
        }

        creditCard.Update(request.CardHolderName, request.ExpiryMonth, request.ExpiryYear);

        var updated = await _creditCardRepository.UpdateAsync(creditCard);

        _logger.LogInformation("✅ Credit card updated: {CardId}", request.Id);

        return _mapper.Map<CreditCardDto>(updated);
    }
}
