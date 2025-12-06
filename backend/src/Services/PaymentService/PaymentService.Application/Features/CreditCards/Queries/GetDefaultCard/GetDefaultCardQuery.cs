using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Features.CreditCards.Queries.GetDefaultCard;

public class GetDefaultCardQuery : IRequest<CreditCardDto?>
{
    public string UserId { get; set; } = string.Empty;
}
