using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;
using TechnologyStore.Shared.Events.Payments;

namespace OrderService.API.Consumers;

/// Consumer for PaymentSuccessEvent from PaymentService.
/// Updates order status to Processing when payment is successful.
public class PaymentSuccessConsumer : IConsumer<IPaymentSuccessEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentSuccessConsumer> _logger;

    public PaymentSuccessConsumer(IMediator mediator, ILogger<PaymentSuccessConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IPaymentSuccessEvent> context)
    {
        var paymentEvent = context.Message;

        _logger.LogInformation("💰 Payment success event received for OrderId: {OrderId}, PaymentIntentId: {PaymentIntentId}",
            paymentEvent.OrderId, paymentEvent.PaymentIntentId);

        try
        {
            // Update order status to PaymentReceived (Ödeme alındı, sipariş hazırlanacak)
            var command = new UpdateOrderStatusCommand
            {
                OrderId = paymentEvent.OrderId,
                Status = "PaymentReceived" // Payment successful, order payment confirmed
            };

            var result = await _mediator.Send(command);

            if (result)
            {
                _logger.LogInformation("✅ Order status updated to PaymentReceived. OrderId: {OrderId}, PaymentIntentId: {PaymentIntentId}",
                    paymentEvent.OrderId, paymentEvent.PaymentIntentId);
            }
            else
            {
                _logger.LogWarning("⚠️ Failed to update order status. Order not found: {OrderId}", paymentEvent.OrderId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to process payment success event for OrderId: {OrderId}", paymentEvent.OrderId);
            throw; // RabbitMQ will retry based on retry policy
        }
    }
}
