using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;
using TechnologyStore.Shared.Events.Payments;

namespace OrderService.API.Consumers;

/// Consumer for PaymentFailedEvent from PaymentService.
/// Updates order status to Failed when payment fails.
public class PaymentFailedConsumer : IConsumer<IPaymentFailedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentFailedConsumer> _logger;

    public PaymentFailedConsumer(IMediator mediator, ILogger<PaymentFailedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IPaymentFailedEvent> context)
    {
        var paymentEvent = context.Message;

        _logger.LogInformation("💳❌ Payment failed event received for OrderId: {OrderId}, Reason: {Reason}",
            paymentEvent.OrderId, paymentEvent.Reason);

        try
        {
            // Update order status to Failed
            var command = new UpdateOrderStatusCommand
            {
                OrderId = paymentEvent.OrderId,
                Status = "Failed" 
            };

            var result = await _mediator.Send(command);

            if (result)
            {
                _logger.LogInformation("✅ Order status updated to Failed. OrderId: {OrderId}, Reason: {Reason}",
                    paymentEvent.OrderId, paymentEvent.Reason);
            }
            else
            {
                _logger.LogWarning("⚠️ Failed to update order status. Order not found: {OrderId}", paymentEvent.OrderId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to process payment failed event for OrderId: {OrderId}", paymentEvent.OrderId);
            throw; // RabbitMQ will retry based on retry policy
        }
    }
}
