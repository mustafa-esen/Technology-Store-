using FluentAssertions;
using ProductService.Application.Features.Categories.Commands.CreateCategory;

namespace ProductService.Tests.Validators.Categories;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator;

    public CreateCategoryCommandValidatorTests()
    {
        _validator = new CreateCategoryCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new CreateCategoryCommand(
            "Electronics",
            "Electronic devices and accessories"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveValidationError()
    {
        var command = new CreateCategoryCommand("", "Description");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public void Validate_NameTooLong_ShouldHaveValidationError()
    {
        var command = new CreateCategoryCommand(
            new string('A', 101),
            "Description"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("100"));
    }

    [Fact]
    public void Validate_EmptyDescription_ShouldHaveValidationError()
    {
        var command = new CreateCategoryCommand("Electronics", "");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_DescriptionTooLong_ShouldHaveValidationError()
    {
        var command = new CreateCategoryCommand(
            "Electronics",
            new string('A', 501)
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("500"));
    }

    [Fact]
    public void Validate_NameAtMaxLength_ShouldBeValid()
    {
        var command = new CreateCategoryCommand(
            new string('A', 100),
            "Description"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_DescriptionAtMaxLength_ShouldBeValid()
    {
        var command = new CreateCategoryCommand(
            "Electronics",
            new string('A', 500)
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
