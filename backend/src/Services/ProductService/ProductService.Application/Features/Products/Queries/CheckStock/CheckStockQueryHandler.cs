using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Products.Queries.CheckStock;

public class CheckStockQueryHandler : IRequestHandler<CheckStockQuery, CheckStockResult>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CheckStockQueryHandler> _logger;

    public CheckStockQueryHandler(
        IProductRepository productRepository,
        ILogger<CheckStockQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<CheckStockResult> Handle(CheckStockQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("📦 Checking stock for {ItemCount} items", request.Items.Count);

        var result = new CheckStockResult { IsAvailable = true };

        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);

            if (product == null)
            {
                result.IsAvailable = false;
                result.Issues.Add(new StockIssue
                {
                    ProductId = item.ProductId,
                    ProductName = "Unknown Product",
                    RequiredQuantity = item.RequiredQuantity,
                    AvailableStock = 0
                });
                _logger.LogWarning("⚠️ Product not found: {ProductId}", item.ProductId);
                continue;
            }

            if (product.Stock < item.RequiredQuantity)
            {
                result.IsAvailable = false;
                result.Issues.Add(new StockIssue
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    RequiredQuantity = item.RequiredQuantity,
                    AvailableStock = product.Stock
                });
                _logger.LogWarning("⚠️ Insufficient stock for '{ProductName}'. Required: {Required}, Available: {Available}",
                    product.Name, item.RequiredQuantity, product.Stock);
            }
        }

        if (result.IsAvailable)
        {
            _logger.LogInformation("✅ All items are available in stock");
        }
        else
        {
            _logger.LogWarning("❌ Stock check failed. {IssueCount} items have insufficient stock", result.Issues.Count);
        }

        return result;
    }
}
