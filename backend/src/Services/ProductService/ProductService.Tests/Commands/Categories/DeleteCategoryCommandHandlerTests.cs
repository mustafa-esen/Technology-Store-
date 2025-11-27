using FluentAssertions;
using Moq;
using ProductService.Application.Features.Categories.Commands.DeleteCategory;
using ProductService.Application.Interfaces;

namespace ProductService.Tests.Commands.Categories;

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new DeleteCategoryCommandHandler(_categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingCategory_ShouldDeleteAndReturnTrue()
    {
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(categoryId))
            .ReturnsAsync(true);

        _categoryRepositoryMock
            .Setup(x => x.DeleteAsync(categoryId))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _categoryRepositoryMock.Verify(x => x.DeleteAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingCategory_ShouldReturnFalse()
    {
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(categoryId))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _categoryRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyGuid_ShouldReturnFalse()
    {
        var command = new DeleteCategoryCommand(Guid.Empty);

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(Guid.Empty))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
