using FluentAssertions;
using FluentValidation.TestHelper;
using OrderService.Application.DTOs;
using OrderService.Application.Features.Orders.Commands.CreateOrder;

namespace OrderService.Tests.Validators;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator;

    public CreateOrderCommandValidatorTests()
    {
        _validator = new CreateOrderCommandValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WithValidCommand()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            UserId = "user-123",
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    Price = 100m,
                    Quantity = 1
                }
            },
            ShippingAddress = new CreateAddressDto
            {
                Street = "123 Main St",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                Country = "USA"
            }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UserId = string.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("UserId is required");
    }

    [Fact]
    public void Validate_ShouldFail_WhenItemsAreEmpty()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Items = new List<CreateOrderItemDto>();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("Order must contain at least one item");
    }

    [Fact]
    public void Validate_ShouldFail_WhenProductIdIsEmpty()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Items[0].ProductId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].ProductId")
            .WithErrorMessage("ProductId is required");
    }

    [Fact]
    public void Validate_ShouldFail_WhenProductNameIsEmpty()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Items[0].ProductName = string.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].ProductName");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_ShouldFail_WhenPriceIsInvalid(decimal price)
    {
        // Arrange
        var command = CreateValidCommand();
        command.Items[0].Price = price;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].Price")
            .WithErrorMessage("Price must be greater than 0");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_ShouldFail_WhenQuantityIsInvalid(int quantity)
    {
        // Arrange
        var command = CreateValidCommand();
        command.Items[0].Quantity = quantity;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].Quantity")
            .WithErrorMessage("Quantity must be greater than 0");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenStreetIsEmpty(string? street)
    {
        // Arrange
        var command = CreateValidCommand();
        command.ShippingAddress.Street = street!;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("ShippingAddress.Street");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenCityIsEmpty(string? city)
    {
        // Arrange
        var command = CreateValidCommand();
        command.ShippingAddress.City = city!;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("ShippingAddress.City");
    }

    [Fact]
    public void Validate_ShouldFail_WhenProductNameExceedsMaxLength()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Items[0].ProductName = new string('A', 201);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].ProductName")
            .WithErrorMessage("ProductName cannot exceed 200 characters");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNotesExceedMaxLength()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Notes = new string('A', 501);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Notes)
            .WithErrorMessage("Notes cannot exceed 500 characters");
    }

    private static CreateOrderCommand CreateValidCommand()
    {
        return new CreateOrderCommand
        {
            UserId = "user-123",
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    Price = 100m,
                    Quantity = 1
                }
            },
            ShippingAddress = new CreateAddressDto
            {
                Street = "123 Main St",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                Country = "USA"
            }
        };
    }
}
