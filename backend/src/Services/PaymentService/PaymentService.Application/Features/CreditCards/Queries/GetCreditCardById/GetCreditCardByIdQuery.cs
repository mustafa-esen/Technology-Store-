using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Features.CreditCards.Queries.GetCreditCardById;

public class GetCreditCardByIdQuery : IRequest<CreditCardDto?>
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
}
