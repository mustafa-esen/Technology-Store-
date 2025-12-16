using AutoMapper;
using FluentAssertions;
using Moq;
using ProductService.Application.DTOs;
using ProductService.Application.Features.Categories.Queries.GetAllCategories;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Tests.Queries.Categories;

public class GetAllCategoriesQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllCategoriesQueryHandler _handler;

    public GetAllCategoriesQueryHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllCategoriesQueryHandler(_categoryRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_CategoriesExist_ShouldReturnCategoryDtos()
    {
        var categories = new List<Category>
        {
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Electronics",
                Description = "Electronic devices",
                IsActive = true
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Books",
                Description = "Books and magazines",
                IsActive = true
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Clothing",
                Description = "Apparel and accessories",
                IsActive = false
            }
        };

        var categoryDtos = categories.Select(c => new CategoryDto(
            c.Id,
            c.Name,
            c.Description,
            c.IsActive
        )).ToList();

        _categoryRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(categories);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<CategoryDto>>(categories))
            .Returns(categoryDtos);

        var query = new GetAllCategoriesQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.First().Name.Should().Be("Electronics");
        result.ElementAt(1).Name.Should().Be("Books");
        result.Last().Name.Should().Be("Clothing");
    }

    [Fact]
    public async Task Handle_NoCategories_ShouldReturnEmptyList()
    {
        _categoryRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Category>());

        _mapperMock
            .Setup(x => x.Map<IEnumerable<CategoryDto>>(It.IsAny<List<Category>>()))
            .Returns(new List<CategoryDto>());

        var query = new GetAllCategoriesQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_OnlyActiveCategories_ShouldReturnAll()
    {
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Active1", IsActive = true },
            new Category { Id = Guid.NewGuid(), Name = "Active2", IsActive = true }
        };

        var categoryDtos = categories.Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IsActive)).ToList();

        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
        _mapperMock.Setup(x => x.Map<IEnumerable<CategoryDto>>(categories)).Returns(categoryDtos);

        var query = new GetAllCategoriesQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
        result.All(c => c.IsActive).Should().BeTrue();
    }
}
