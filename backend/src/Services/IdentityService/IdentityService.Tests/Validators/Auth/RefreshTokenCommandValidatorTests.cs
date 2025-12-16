using FluentAssertions;
using IdentityService.Application.Features.Auth.Commands.RefreshToken;

namespace IdentityService.Tests.Validators.Auth;

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator;

    public RefreshTokenCommandValidatorTests()
    {
        _validator = new RefreshTokenCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new RefreshTokenCommand("valid_refresh_token_string");

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
    public void Validate_EmptyRefreshToken_ShouldHaveValidationError(string refreshToken)
    {
        // Arrange
        var command = new RefreshTokenCommand(refreshToken);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RefreshToken");
    }

    [Fact]
    public void Validate_ValidRefreshToken_ShouldPass()
    {
        // Arrange
        var command = new RefreshTokenCommand("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
