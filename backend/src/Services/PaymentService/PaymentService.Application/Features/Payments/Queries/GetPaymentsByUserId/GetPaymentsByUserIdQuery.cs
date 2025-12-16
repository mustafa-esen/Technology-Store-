using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Features.Payments.Queries.GetPaymentsByUserId;

public class GetPaymentsByUserIdQuery : IRequest<List<PaymentDto>>
{
    public string UserId { get; set; } = string.Empty;
}
