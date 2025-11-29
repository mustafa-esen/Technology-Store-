using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BasketService.Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<BasketRepository> _logger;
    private const string BasketKeyPrefix = "basket:";

    public BasketRepository(IConnectionMultiplexer redis, ILogger<BasketRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<Basket?> GetBasketAsync(string userId)
    {
        try
        {
            _logger.LogInformation("🔍 Fetching basket for user: {UserId}", userId);

            var data = await _database.StringGetAsync(GetBasketKey(userId));

            if (data.IsNullOrEmpty)
            {
                _logger.LogInformation("📭 No basket found for user: {UserId}", userId);
                return null;
            }

            var basket = JsonConvert.DeserializeObject<Basket>(data!);
            _logger.LogInformation("✅ Basket retrieved for user: {UserId} with {ItemCount} items",
                userId, basket?.Items.Count ?? 0);

            return basket;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving basket for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<Basket> SaveBasketAsync(Basket basket)
    {
        try
        {
            _logger.LogInformation("💾 Saving basket for user: {UserId} with {ItemCount} items",
                basket.UserId, basket.Items.Count);

            var serializedBasket = JsonConvert.SerializeObject(basket);

            var created = await _database.StringSetAsync(
                GetBasketKey(basket.UserId),
                serializedBasket,
                TimeSpan.FromDays(30)
            );

            if (!created)
            {
                _logger.LogError("❌ Failed to save basket to Redis for user: {UserId}", basket.UserId);
                throw new Exception("Failed to save basket to Redis");
            }

            _logger.LogInformation("✅ Basket saved successfully for user: {UserId}, Total: {TotalPrice:C}",
                basket.UserId, basket.TotalPrice);

            return basket;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error saving basket for user: {UserId}", basket.UserId);
            throw;
        }
    }

    public async Task<bool> DeleteBasketAsync(string userId)
    {
        try
        {
            _logger.LogInformation("🗑️ Deleting basket for user: {UserId}", userId);

            var result = await _database.KeyDeleteAsync(GetBasketKey(userId));

            if (result)
            {
                _logger.LogInformation("✅ Basket deleted successfully for user: {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("⚠️ No basket found to delete for user: {UserId}", userId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error deleting basket for user: {UserId}", userId);
            throw;
        }
    }

    private static string GetBasketKey(string userId) => $"{BasketKeyPrefix}{userId}";
}
