using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Features.Payments.Queries.GetPaymentById;

public class GetPaymentByIdQuery : IRequest<PaymentDto?>
{
    public Guid Id { get; set; }
}
