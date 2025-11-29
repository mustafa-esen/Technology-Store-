using BasketService.Application.Features.Baskets.Commands.AddItemToBasket;
using FluentAssertions;

namespace BasketService.Tests.Validators;

public class AddItemToBasketCommandValidatorTests
{
    private readonly AddItemToBasketCommandValidator _validator;

    public AddItemToBasketCommandValidatorTests()
    {
        _validator = new AddItemToBasketCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Valid Product",
            Price = 100,
            Quantity = 1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyProductId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.Empty,
            ProductName = "Product",
            Price = 100,
            Quantity = 1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
    }

    [Fact]
    public void Validate_WithEmptyProductName_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = string.Empty,
            Price = 100,
            Quantity = 1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductName");
    }

    [Fact]
    public void Validate_WithNullProductName_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = null!,
            Price = 100,
            Quantity = 1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductName");
    }

    [Fact]
    public void Validate_WithProductNameExceeding200Characters_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = new string('A', 201),
            Price = 100,
            Quantity = 1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductName" && e.ErrorMessage.Contains("200"));
    }

    [Fact]
    public void Validate_WithProductNameExactly200Characters_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = new string('A', 200),
            Price = 100,
            Quantity = 1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithZeroPrice_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = 0,
            Quantity = 1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_WithNegativePrice_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = -10,
            Quantity = 1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price" && e.ErrorMessage.Contains("greater than 0"));
    }

    [Fact]
    public void Validate_WithZeroQuantity_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = 100,
            Quantity = 0
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity");
    }

    [Fact]
    public void Validate_WithNegativeQuantity_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = 100,
            Quantity = -5
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity" && e.ErrorMessage.Contains("greater than 0"));
    }

    [Fact]
    public void Validate_WithMultipleErrors_ShouldHaveAllValidationErrors()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.Empty,
            ProductName = string.Empty,
            Price = 0,
            Quantity = 0
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(4);
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
        result.Errors.Should().Contain(e => e.PropertyName == "ProductName");
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity");
    }

    [Fact]
    public void Validate_WithLargeValidValues_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Expensive Product",
            Price = 999999.99m,
            Quantity = 1000
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(10000)]
    public void Validate_WithValidPrices_ShouldNotHaveValidationError(decimal price)
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = price,
            Quantity = 1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void Validate_WithValidQuantities_ShouldNotHaveValidationError(int quantity)
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = 100,
            Quantity = quantity
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
