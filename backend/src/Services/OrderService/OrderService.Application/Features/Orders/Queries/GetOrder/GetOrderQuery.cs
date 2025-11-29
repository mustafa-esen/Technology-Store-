using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetOrder;

public class GetOrderQuery : IRequest<OrderDto?>
{
    public Guid OrderId { get; set; }
}
