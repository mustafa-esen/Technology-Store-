using AutoMapper;
using FluentAssertions;
using Moq;
using ProductService.Application.DTOs;
using ProductService.Application.Features.Categories.Queries.GetCategoryById;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Tests.Queries.Categories;

public class GetCategoryByIdQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetCategoryByIdQueryHandler _handler;

    public GetCategoryByIdQueryHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetCategoryByIdQueryHandler(_categoryRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_CategoryExists_ShouldReturnCategoryDto()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Electronics",
            Description = "Electronic devices and accessories",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var categoryDto = new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.IsActive
        );

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _mapperMock
            .Setup(x => x.Map<CategoryDto>(category))
            .Returns(categoryDto);

        var query = new GetCategoryByIdQuery(categoryId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(categoryId);
        result.Name.Should().Be("Electronics");
        result.Description.Should().Be("Electronic devices and accessories");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ShouldReturnNull()
    {
        var categoryId = Guid.NewGuid();

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        var query = new GetCategoryByIdQuery(categoryId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
        _mapperMock.Verify(x => x.Map<CategoryDto>(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InactiveCategory_ShouldStillReturn()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Inactive", IsActive = false };
        var categoryDto = new CategoryDto(categoryId, "Inactive", "", false);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _mapperMock.Setup(x => x.Map<CategoryDto>(category)).Returns(categoryDto);

        var query = new GetCategoryByIdQuery(categoryId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.IsActive.Should().BeFalse();
    }
}
