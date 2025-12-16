using ReviewService.Domain.Entities;

namespace ReviewService.Application.Interfaces;

public interface IReviewRepository
{
    Task<ProductReview?> GetByIdAsync(string id);
    Task<List<ProductReview>> GetAllAsync();
    Task<List<ProductReview>> GetByProductIdAsync(Guid productId);
    Task<List<ProductReview>> GetByUserIdAsync(Guid userId);
    Task<ProductReview> CreateAsync(ProductReview review);
    Task<bool> UpdateAsync(string id, ProductReview review);
    Task<bool> DeleteAsync(string id);
}
