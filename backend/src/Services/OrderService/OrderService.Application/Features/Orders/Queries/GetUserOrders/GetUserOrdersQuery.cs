using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetUserOrders;

public class GetUserOrdersQuery : IRequest<List<OrderDto>>
{
    public string UserId { get; set; } = string.Empty;
}
