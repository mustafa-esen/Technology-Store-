using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQuery : IRequest<List<OrderDto>>
{
}
