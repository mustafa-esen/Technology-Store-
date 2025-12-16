using FluentAssertions;
using ProductService.Application.Features.Products.Commands.CreateProduct;

namespace ProductService.Tests.Validators;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new CreateProductCommand(
            "iPhone 15 Pro",
            "Latest Apple smartphone with titanium design",
            999.99m,
            50,
            Guid.NewGuid(),
            "Apple",
            "https://example.com/iphone.jpg"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveValidationError()
    {
        var command = new CreateProductCommand(
            "",
            "Description",
            100m,
            10,
            Guid.NewGuid(),
            "Brand",
            "image.jpg"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameTooLong_ShouldHaveValidationError()
    {
        var command = new CreateProductCommand(
            new string('A', 201),
            "Description",
            100m,
            10,
            Guid.NewGuid(),
            "Brand",
            "image.jpg"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NegativePrice_ShouldHaveValidationError()
    {
        var command = new CreateProductCommand(
            "Product",
            "Description",
            -10m,
            10,
            Guid.NewGuid(),
            "Brand",
            "image.jpg"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_ZeroPrice_ShouldHaveValidationError()
    {
        var command = new CreateProductCommand(
            "Product",
            "Description",
            0m,
            10,
            Guid.NewGuid(),
            "Brand",
            "image.jpg"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_NegativeStock_ShouldHaveValidationError()
    {
        var command = new CreateProductCommand(
            "Product",
            "Description",
            100m,
            -5,
            Guid.NewGuid(),
            "Brand",
            "image.jpg"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Stock");
    }

    [Fact]
    public void Validate_ZeroStock_ShouldBeValid()
    {
        var command = new CreateProductCommand(
            "Product",
            "Description",
            100m,
            0,
            Guid.NewGuid(),
            "Brand",
            "image.jpg"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyCategoryId_ShouldHaveValidationError()
    {
        var command = new CreateProductCommand(
            "Product",
            "Description",
            100m,
            10,
            Guid.Empty,
            "Brand",
            "image.jpg"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
    }
}
