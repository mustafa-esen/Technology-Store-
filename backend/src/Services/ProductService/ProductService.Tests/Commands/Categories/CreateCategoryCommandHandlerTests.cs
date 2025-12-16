using AutoMapper;
using FluentAssertions;
using Moq;
using ProductService.Application.DTOs;
using ProductService.Application.Features.Categories.Commands.CreateCategory;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Tests.Commands.Categories;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCategory()
    {
        var command = new CreateCategoryCommand(
            "Electronics",
            "Electronic devices and accessories"
        );

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var categoryDto = new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.IsActive
        );

        _mapperMock
            .Setup(x => x.Map<Category>(command))
            .Returns(category);

        _categoryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync(category);

        _mapperMock
            .Setup(x => x.Map<CategoryDto>(category))
            .Returns(categoryDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Electronics");
        result.Description.Should().Be("Electronic devices and accessories");
        result.IsActive.Should().BeTrue();
        
        _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnCategoryWithId()
    {
        var command = new CreateCategoryCommand("Books", "Books and magazines");
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Books", Description = "Books and magazines" };
        var categoryDto = new CategoryDto(categoryId, "Books", "Books and magazines", true);

        _mapperMock.Setup(x => x.Map<Category>(command)).Returns(category);
        _categoryRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Category>())).ReturnsAsync(category);
        _mapperMock.Setup(x => x.Map<CategoryDto>(category)).Returns(categoryDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(categoryId);
        result.Id.Should().NotBe(Guid.Empty);
    }
}
