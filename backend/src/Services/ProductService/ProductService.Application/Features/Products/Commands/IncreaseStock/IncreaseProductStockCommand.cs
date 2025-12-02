using MediatR;

namespace ProductService.Application.Features.Products.Commands.IncreaseStock;

public class IncreaseProductStockCommand : IRequest<bool>
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
