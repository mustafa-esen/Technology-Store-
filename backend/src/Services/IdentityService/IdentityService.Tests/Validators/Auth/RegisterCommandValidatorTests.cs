using FluentAssertions;
using IdentityService.Application.Features.Auth.Commands.Register;

namespace IdentityService.Tests.Validators.Auth;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new RegisterCommand(
            "test@example.com",
            "SecurePass123!",
            "John",
            "Doe",
            null
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_EmptyEmail_ShouldHaveValidationError(string email)
    {
        // Arrange
        var command = new RegisterCommand(email, "Pass123!", "John", "Doe", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test.example.com")]
    public void Validate_InvalidEmailFormat_ShouldHaveValidationError(string email)
    {
        // Arrange
        var command = new RegisterCommand(email, "Pass123!", "John", "Doe", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_EmptyPassword_ShouldHaveValidationError(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password, "John", "Doe", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567")]
    public void Validate_PasswordTooShort_ShouldHaveValidationError(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password, "John", "Doe", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password" && e.ErrorMessage.Contains("8"));
    }

    [Theory]
    [InlineData("alllowercase")]
    [InlineData("lowercase123")]
    [InlineData("lowercase!@#")]
    public void Validate_PasswordNoUppercase_ShouldHaveValidationError(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password, "John", "Doe", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("ALLUPPERCASE")]
    [InlineData("UPPERCASE123")]
    [InlineData("UPPERCASE!@#")]
    public void Validate_PasswordNoLowercase_ShouldHaveValidationError(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password, "John", "Doe", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("NoNumbers!")]
    [InlineData("OnlyLetters!@#")]
    public void Validate_PasswordNoNumber_ShouldHaveValidationError(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password, "John", "Doe", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("NoSpecialChar123")]
    [InlineData("OnlyLetters123")]
    public void Validate_PasswordNoSpecialCharacter_ShouldHaveValidationError(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password, "John", "Doe", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_EmptyFirstName_ShouldHaveValidationError(string firstName)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Pass123!", firstName, "Doe", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_EmptyLastName_ShouldHaveValidationError(string lastName)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Pass123!", "John", lastName, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    [Fact]
    public void Validate_FirstNameTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new RegisterCommand("test@example.com", "Pass123!", longName, "Doe", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName" && e.ErrorMessage.Contains("100"));
    }

    [Fact]
    public void Validate_LastNameTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new RegisterCommand("test@example.com", "Pass123!", "John", longName, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName" && e.ErrorMessage.Contains("100"));
    }
}
