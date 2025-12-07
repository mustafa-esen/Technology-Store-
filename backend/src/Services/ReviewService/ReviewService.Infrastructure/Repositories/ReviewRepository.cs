using MongoDB.Driver;
using ReviewService.Application.Interfaces;
using ReviewService.Domain.Entities;
using ReviewService.Infrastructure.Data;

namespace ReviewService.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly MongoDbContext _context;

    public ReviewRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<ProductReview?> GetByIdAsync(string id)
    {
        return await _context.Reviews
            .Find(r => r.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ProductReview>> GetByProductIdAsync(Guid productId)
    {
        return await _context.Reviews
            .Find(r => r.ProductId == productId)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ProductReview>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Reviews
            .Find(r => r.UserId == userId)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<ProductReview> CreateAsync(ProductReview review)
    {
        await _context.Reviews.InsertOneAsync(review);
        return review;
    }

    public async Task<bool> UpdateAsync(string id, ProductReview review)
    {
        review.UpdatedAt = DateTime.UtcNow;
        var result = await _context.Reviews
            .ReplaceOneAsync(r => r.Id == id, review);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _context.Reviews
            .DeleteOneAsync(r => r.Id == id);
        return result.DeletedCount > 0;
    }
}
