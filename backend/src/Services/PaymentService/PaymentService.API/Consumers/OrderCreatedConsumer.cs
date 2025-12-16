using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Features.Payments.Commands.ProcessPayment;
using TechnologyStore.Shared.Events.Orders;

namespace PaymentService.API.Consumers;

/// <summary>
/// Consumer for OrderCreatedEvent from OrderService.
/// This acts as the entry point (like a Controller) for event-driven architecture.
/// </summary>
public class OrderCreatedConsumer : IConsumer<IOrderCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(IMediator mediator, ILogger<OrderCreatedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
    {
        var orderEvent = context.Message;

        _logger.LogInformation("🛒 Order created event received for OrderId: {OrderId}, UserId: {UserId}, Amount: {Amount}",
            orderEvent.OrderId, orderEvent.UserId, orderEvent.TotalAmount);

        try
        {
            // Convert event to command
            var command = new ProcessPaymentCommand
            {
                OrderId = orderEvent.OrderId.ToString(),
                UserId = orderEvent.UserId,
                TotalAmount = orderEvent.TotalAmount,
                Currency = "TRY"
            };

            // Process payment through MediatR pipeline
            var result = await _mediator.Send(command);

            _logger.LogInformation("✅ Payment processed successfully for OrderId: {OrderId}, PaymentId: {PaymentId}, Status: {Status}",
                orderEvent.OrderId, result.Id, result.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to process payment for OrderId: {OrderId}", orderEvent.OrderId);
            throw; // RabbitMQ will retry based on retry policy
        }
    }
}
