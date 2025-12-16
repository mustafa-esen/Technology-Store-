using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
