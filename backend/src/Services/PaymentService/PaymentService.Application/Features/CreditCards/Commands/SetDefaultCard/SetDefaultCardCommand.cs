using MediatR;

namespace PaymentService.Application.Features.CreditCards.Commands.SetDefaultCard;

public class SetDefaultCardCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
}
