using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
