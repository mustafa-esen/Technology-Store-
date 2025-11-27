using MediatR;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        if (!await _productRepository.ExistsAsync(request.Id))
            return false;

        await _productRepository.DeleteAsync(request.Id);
        return true;
    }
}
