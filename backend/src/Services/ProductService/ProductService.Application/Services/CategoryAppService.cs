using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Application.Services;

public class CategoryAppService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryAppService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category == null ? null : MapToDto(category);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _categoryRepository.AddAsync(category);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return false;

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.IsActive = dto.IsActive;

        await _categoryRepository.UpdateAsync(category);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _categoryRepository.ExistsAsync(id))
            return false;

        await _categoryRepository.DeleteAsync(id);
        return true;
    }

    private static CategoryDto MapToDto(Category category) => new(
        category.Id,
        category.Name,
        category.Description,
        category.IsActive
    );
}
