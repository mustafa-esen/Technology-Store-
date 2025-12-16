using AutoMapper;
using MediatR;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;

namespace PaymentService.Application.Features.Payments.Queries.GetAllPayments;

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, List<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetAllPaymentsQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<List<PaymentDto>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetAllAsync();
        return _mapper.Map<List<PaymentDto>>(payments);
    }
}
