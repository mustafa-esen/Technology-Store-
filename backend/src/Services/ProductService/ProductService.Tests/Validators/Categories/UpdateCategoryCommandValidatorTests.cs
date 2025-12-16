using FluentAssertions;
using ProductService.Application.Features.Categories.Commands.UpdateCategory;

namespace ProductService.Tests.Validators.Categories;

public class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator;

    public UpdateCategoryCommandValidatorTests()
    {
        _validator = new UpdateCategoryCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            "Updated Electronics",
            "Updated description",
            true
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyId_ShouldHaveValidationError()
    {
        var command = new UpdateCategoryCommand(
            Guid.Empty,
            "Name",
            "Description",
            true
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveValidationError()
    {
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            "",
            "Description",
            true
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameTooLong_ShouldHaveValidationError()
    {
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            new string('A', 101),
            "Description",
            true
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_EmptyDescription_ShouldHaveValidationError()
    {
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            "Electronics",
            "",
            true
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_IsActiveFalse_ShouldBeValid()
    {
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            "Electronics",
            "Description",
            false
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
