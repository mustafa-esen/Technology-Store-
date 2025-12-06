using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Features.CreditCards.Commands.UpdateCreditCard;

public class UpdateCreditCardCommand : IRequest<CreditCardDto>
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
}
