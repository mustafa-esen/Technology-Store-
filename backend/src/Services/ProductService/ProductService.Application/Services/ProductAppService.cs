using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Application.Services;

public class ProductAppService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductAppService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(Guid categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
        {
            throw new ArgumentException($"Category with ID {dto.CategoryId} not found");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            Brand = dto.Brand,
            ImageUrl = dto.ImageUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _productRepository.AddAsync(product);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return false;

        if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
        {
            throw new ArgumentException($"Category with ID {dto.CategoryId} not found");
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.CategoryId = dto.CategoryId;
        product.Brand = dto.Brand;
        product.ImageUrl = dto.ImageUrl;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _productRepository.ExistsAsync(id))
            return false;

        await _productRepository.DeleteAsync(id);
        return true;
    }

    private static ProductDto MapToDto(Product product) => new(
        product.Id,
        product.Name,
        product.Description,
        product.Price,
        product.Stock,
        product.CategoryId,
        product.Category?.Name ?? "Unknown",
        product.Brand,
        product.ImageUrl,
        product.IsActive
    );
}
