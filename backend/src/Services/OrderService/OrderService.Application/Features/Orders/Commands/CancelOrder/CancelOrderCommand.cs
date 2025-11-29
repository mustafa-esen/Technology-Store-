using MediatR;

namespace OrderService.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommand : IRequest<bool>
{
    public Guid OrderId { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
}
