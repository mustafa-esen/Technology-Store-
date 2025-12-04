using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Interfaces;

namespace PaymentService.Application.Features.CreditCards.Commands.SetDefaultCard;

public class SetDefaultCardCommandHandler : IRequestHandler<SetDefaultCardCommand, bool>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ILogger<SetDefaultCardCommandHandler> _logger;

    public SetDefaultCardCommandHandler(
        ICreditCardRepository creditCardRepository,
        ILogger<SetDefaultCardCommandHandler> logger)
    {
        _creditCardRepository = creditCardRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(SetDefaultCardCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("⭐ Setting default card: {CardId} for user: {UserId}",
            request.Id, request.UserId);

        var creditCard = await _creditCardRepository.GetByIdAsync(request.Id);

        if (creditCard == null)
        {
            _logger.LogWarning("⚠️ Credit card not found: {CardId}", request.Id);
            return false;
        }

        if (creditCard.UserId != request.UserId)
        {
            _logger.LogWarning("⚠️ User {UserId} tried to set default for card owned by {OwnerId}",
                request.UserId, creditCard.UserId);
            return false;
        }

        // Kullanıcının tüm kartlarını al ve varsayılanı kaldır
        var userCards = await _creditCardRepository.GetByUserIdAsync(request.UserId);
        foreach (var card in userCards)
        {
            if (card.IsDefault)
            {
                card.RemoveDefault();
                await _creditCardRepository.UpdateAsync(card);
            }
        }

        // Bu kartı varsayılan yap
        creditCard.SetAsDefault();
        await _creditCardRepository.UpdateAsync(creditCard);

        _logger.LogInformation("✅ Default card set: {CardId}", request.Id);

        return true;
    }
}
