using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Features.Payments.Queries.GetAllPayments;

public class GetAllPaymentsQuery : IRequest<List<PaymentDto>>
{
}
