using System.Text;
using System.Text.Json;
using BasketService.Application.DTOs;
using BasketService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BasketService.Infrastructure.Services;

public class ProductServiceClient : IProductServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductServiceClient> _logger;
    private readonly string _productServiceUrl;

    public ProductServiceClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ProductServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _productServiceUrl = configuration["ProductService:Url"] ?? "http://localhost:5000";
    }

    public async Task<StockCheckResponse> CheckStockAsync(StockCheckRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("📡 Calling ProductService stock check for {ItemCount} items", request.Items.Count);

            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_productServiceUrl}/api/stock/check", content, cancellationToken);

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠️ ProductService stock check returned {StatusCode}", response.StatusCode);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var result = JsonSerializer.Deserialize<StockCheckResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result ?? new StockCheckResponse { IsAvailable = false };
                }

                return new StockCheckResponse { IsAvailable = false };
            }

            var stockResponse = JsonSerializer.Deserialize<StockCheckResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("✅ Stock check completed. Available: {IsAvailable}", stockResponse?.IsAvailable ?? false);

            return stockResponse ?? new StockCheckResponse { IsAvailable = false };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error calling ProductService stock check");
            return new StockCheckResponse { IsAvailable = false };
        }
    }
}
