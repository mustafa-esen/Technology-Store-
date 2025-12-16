using BasketService.Application.Features.Baskets.Commands.UpdateItemQuantity;
using FluentAssertions;

namespace BasketService.Tests.Validators;

public class UpdateItemQuantityCommandValidatorTests
{
    private readonly UpdateItemQuantityCommandValidator _validator;

    public UpdateItemQuantityCommandValidatorTests()
    {
        _validator = new UpdateItemQuantityCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateItemQuantityCommand
        {
            UserId = "user-123",
            ProductId = Guid.NewGuid(),
            Quantity = 5
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyUserId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateItemQuantityCommand
        {
            UserId = string.Empty,
            ProductId = Guid.NewGuid(),
            Quantity = 5
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public void Validate_WithNullUserId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateItemQuantityCommand
        {
            UserId = null!,
            ProductId = Guid.NewGuid(),
            Quantity = 5
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public void Validate_WithEmptyProductId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateItemQuantityCommand
        {
            UserId = "user-123",
            ProductId = Guid.Empty,
            Quantity = 5
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
    }

    [Fact]
    public void Validate_WithNegativeQuantity_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateItemQuantityCommand
        {
            UserId = "user-123",
            ProductId = Guid.NewGuid(),
            Quantity = -1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity" && e.ErrorMessage.Contains("0 or greater"));
    }

    [Fact]
    public void Validate_WithZeroQuantity_ShouldBeValid()
    {
        // Arrange
        var command = new UpdateItemQuantityCommand
        {
            UserId = "user-123",
            ProductId = Guid.NewGuid(),
            Quantity = 0  // Zero is allowed (>= 0)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMultipleErrors_ShouldHaveAllValidationErrors()
    {
        // Arrange
        var command = new UpdateItemQuantityCommand
        {
            UserId = string.Empty,
            ProductId = Guid.Empty,
            Quantity = -5  // Negative to trigger all 3 errors
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Validate_WithValidQuantities_ShouldNotHaveValidationError(int quantity)
    {
        // Arrange
        var command = new UpdateItemQuantityCommand
        {
            UserId = "user-123",
            ProductId = Guid.NewGuid(),
            Quantity = quantity
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithWhitespaceUserId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateItemQuantityCommand
        {
            UserId = "   ",
            ProductId = Guid.NewGuid(),
            Quantity = 5
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public void Validate_WithLongUserId_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateItemQuantityCommand
        {
            UserId = new string('a', 500),
            ProductId = Guid.NewGuid(),
            Quantity = 5
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
