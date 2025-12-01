using MediatR;

namespace ProductService.Application.Features.Products.Commands.DecreaseStock;

public class DecreaseProductStockCommand : IRequest<bool>
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
