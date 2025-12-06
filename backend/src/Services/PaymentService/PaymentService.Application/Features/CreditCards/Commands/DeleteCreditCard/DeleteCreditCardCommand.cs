using MediatR;

namespace PaymentService.Application.Features.CreditCards.Commands.DeleteCreditCard;

public class DeleteCreditCardCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
}
