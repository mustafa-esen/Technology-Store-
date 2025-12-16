using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Products.Commands.IncreaseStock;

public class IncreaseProductStockCommandHandler : IRequestHandler<IncreaseProductStockCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<IncreaseProductStockCommandHandler> _logger;

    public IncreaseProductStockCommandHandler(
        IProductRepository productRepository,
        ILogger<IncreaseProductStockCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(IncreaseProductStockCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("📦 Increasing stock for ProductId: {ProductId}, Quantity: {Quantity}",
            request.ProductId, request.Quantity);

        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
        {
            _logger.LogWarning("⚠️ Product not found: {ProductId}", request.ProductId);
            return false;
        }

        try
        {
            product.IncreaseStock(request.Quantity);
            await _productRepository.UpdateAsync(product);

            _logger.LogInformation("✅ Stock increased successfully for '{ProductName}'. New stock: {NewStock}",
                product.Name, product.Stock);

            return true;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("❌ Failed to increase stock: {Message}", ex.Message);
            return false;
        }
    }
}
