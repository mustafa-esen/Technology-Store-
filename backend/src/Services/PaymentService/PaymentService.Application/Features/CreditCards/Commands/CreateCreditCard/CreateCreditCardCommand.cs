using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Features.CreditCards.Commands.CreateCreditCard;

public class CreateCreditCardCommand : IRequest<CreditCardDto>
{
    public string UserId { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
}
