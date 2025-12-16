using AutoMapper;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;
using TechnologyStore.Shared.Events.Payments;

namespace PaymentService.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        IPublishEndpoint publishEndpoint,
        IMapper mapper,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _publishEndpoint = publishEndpoint;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaymentDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var existingPayment = await _paymentRepository.GetByOrderIdAsync(request.OrderId);
        if (existingPayment != null)
        {
            _logger.LogWarning("⚠️ Payment already processed for OrderId: {OrderId}. Returning existing payment.", request.OrderId);
            return _mapper.Map<PaymentDto>(existingPayment);
        }

        _logger.LogInformation("💳 Processing payment for OrderId: {OrderId}, Amount: {Amount} {Currency}",
            request.OrderId, request.TotalAmount, request.Currency);

        // Create payment entity
        var money = new Money(request.TotalAmount, request.Currency);
        var payment = new Payment(request.OrderId, request.UserId, money);

        // Mark as processing
        payment.MarkAsProcessing();
        await _paymentRepository.AddAsync(payment);

        // Process payment through gateway (FakePaymentGateway)
        var gatewayResult = await _paymentGateway.ProcessPaymentAsync(
            request.OrderId,
            request.TotalAmount,
            request.UserId);

        if (gatewayResult.IsSuccess)
        {
            // Payment succeeded
            payment.MarkAsSuccess(gatewayResult.TransactionId!);
            await _paymentRepository.UpdateAsync(payment);

            _logger.LogInformation("✅ Payment successful for OrderId: {OrderId}, TransactionId: {TransactionId}",
                request.OrderId, gatewayResult.TransactionId);

            // Publish PaymentSuccessEvent
            await _publishEndpoint.Publish<IPaymentSuccessEvent>(new
            {
                OrderId = payment.OrderId,
                UserId = payment.UserId,
                Amount = payment.Amount.Amount,
                PaymentIntentId = payment.TransactionId,
                PaymentMethod = "CreditCard",
                CompletedDate = payment.ProcessedDate!.Value
            }, cancellationToken);

            _logger.LogInformation("🐰 PaymentSuccessEvent published for OrderId: {OrderId}", request.OrderId);
        }
        else
        {
            // Payment failed
            payment.MarkAsFailed(gatewayResult.FailureReason!);
            await _paymentRepository.UpdateAsync(payment);

            _logger.LogWarning("❌ Payment failed for OrderId: {OrderId}, Reason: {Reason}",
                request.OrderId, gatewayResult.FailureReason);

            // Publish PaymentFailedEvent
            await _publishEndpoint.Publish<IPaymentFailedEvent>(new
            {
                OrderId = payment.OrderId,
                UserId = payment.UserId,
                Amount = payment.Amount.Amount,
                Reason = payment.FailureReason,
                FailedDate = payment.ProcessedDate!.Value
            }, cancellationToken);

            _logger.LogInformation("🐰 PaymentFailedEvent published for OrderId: {OrderId}", request.OrderId);
        }

        return _mapper.Map<PaymentDto>(payment);
    }
}
