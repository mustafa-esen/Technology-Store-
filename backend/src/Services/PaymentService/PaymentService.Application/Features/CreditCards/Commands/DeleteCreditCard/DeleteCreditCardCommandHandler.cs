using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Interfaces;

namespace PaymentService.Application.Features.CreditCards.Commands.DeleteCreditCard;

public class DeleteCreditCardCommandHandler : IRequestHandler<DeleteCreditCardCommand, bool>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ILogger<DeleteCreditCardCommandHandler> _logger;

    public DeleteCreditCardCommandHandler(
        ICreditCardRepository creditCardRepository,
        ILogger<DeleteCreditCardCommandHandler> logger)
    {
        _creditCardRepository = creditCardRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteCreditCardCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🗑️ Deleting credit card: {CardId} for user: {UserId}",
            request.Id, request.UserId);

        var creditCard = await _creditCardRepository.GetByIdAsync(request.Id);

        if (creditCard == null)
        {
            _logger.LogWarning("⚠️ Credit card not found: {CardId}", request.Id);
            return false;
        }

        if (creditCard.UserId != request.UserId)
        {
            _logger.LogWarning("⚠️ User {UserId} tried to delete card owned by {OwnerId}",
                request.UserId, creditCard.UserId);
            return false;
        }

        await _creditCardRepository.DeleteAsync(request.Id);

        _logger.LogInformation("✅ Credit card deleted: {CardId}", request.Id);

        return true;
    }
}
