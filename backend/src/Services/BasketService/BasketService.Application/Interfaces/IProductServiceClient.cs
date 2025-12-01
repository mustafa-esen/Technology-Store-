using BasketService.Application.DTOs;

namespace BasketService.Application.Interfaces;

public interface IProductServiceClient
{
    Task<StockCheckResponse> CheckStockAsync(StockCheckRequest request, CancellationToken cancellationToken = default);
}
