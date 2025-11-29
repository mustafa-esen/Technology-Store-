using BasketService.Domain.Entities;

namespace BasketService.Application.Interfaces;

public interface IBasketRepository
{
    Task<Basket?> GetBasketAsync(string userId);
    Task<Basket> SaveBasketAsync(Basket basket);
    Task<bool> DeleteBasketAsync(string userId);
}
