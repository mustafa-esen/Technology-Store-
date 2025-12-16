using FluentAssertions;
using Moq;
using ProductService.Application.Features.Categories.Commands.UpdateCategory;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Tests.Commands.Categories;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new UpdateCategoryCommandHandler(_categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateCategory()
    {
        var categoryId = Guid.NewGuid();
        var existingCategory = new Category
        {
            Id = categoryId,
            Name = "Old Name",
            Description = "Old Description",
            IsActive = true
        };

        var command = new UpdateCategoryCommand(
            categoryId,
            "Updated Electronics",
            "Updated description for electronics",
            false
        );

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        existingCategory.Name.Should().Be("Updated Electronics");
        existingCategory.Description.Should().Be("Updated description for electronics");
        existingCategory.IsActive.Should().BeFalse();

        _categoryRepositoryMock.Verify(x => x.UpdateAsync(existingCategory), Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ShouldReturnFalse()
    {
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand(
            categoryId,
            "Name",
            "Description",
            true
        );

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _categoryRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeactivateCategory_ShouldSetIsActiveToFalse()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Test", IsActive = true };
        var command = new UpdateCategoryCommand(categoryId, "Test", "Desc", false);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _categoryRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        category.IsActive.Should().BeFalse();
    }
}
