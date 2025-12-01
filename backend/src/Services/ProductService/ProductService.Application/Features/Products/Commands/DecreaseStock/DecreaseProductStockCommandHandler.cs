using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Products.Commands.DecreaseStock;

public class DecreaseProductStockCommandHandler : IRequestHandler<DecreaseProductStockCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DecreaseProductStockCommandHandler> _logger;

    public DecreaseProductStockCommandHandler(
        IProductRepository productRepository,
        ILogger<DecreaseProductStockCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DecreaseProductStockCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("📦 Decreasing stock for ProductId: {ProductId}, Quantity: {Quantity}",
            request.ProductId, request.Quantity);

        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
        {
            _logger.LogWarning("⚠️ Product not found: {ProductId}", request.ProductId);
            return false;
        }

        try
        {
            product.DecreaseStock(request.Quantity);
            await _productRepository.UpdateAsync(product);

            _logger.LogInformation("✅ Stock decreased successfully for '{ProductName}'. New stock: {NewStock}",
                product.Name, product.Stock);

            return true;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError("❌ Failed to decrease stock: {Message}", ex.Message);
            return false;
        }
    }
}
