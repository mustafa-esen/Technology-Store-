using MediatR;

namespace OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommand : IRequest<bool>
{
    public Guid OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
}
