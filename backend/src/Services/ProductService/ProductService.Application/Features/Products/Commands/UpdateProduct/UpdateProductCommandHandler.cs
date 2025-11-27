using MediatR;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null) return false;

        if (!await _categoryRepository.ExistsAsync(request.CategoryId))
        {
            throw new ArgumentException($"Category with ID {request.CategoryId} not found");
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;
        product.Brand = request.Brand;
        product.ImageUrl = request.ImageUrl;
        product.IsActive = request.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        return true;
    }
}
