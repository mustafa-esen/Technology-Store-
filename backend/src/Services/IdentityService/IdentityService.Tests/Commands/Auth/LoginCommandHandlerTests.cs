using FluentAssertions;
using Moq;
using IdentityService.Application.DTOs;
using IdentityService.Application.Features.Auth.Commands.Login;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;

namespace IdentityService.Tests.Commands.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();

        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "SecurePass123!");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            PasswordHash = "hashed_password",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };

        var accessToken = "access_token_jwt";
        var refreshTokenValue = "refresh_token_value";
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenValue,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        var userDto = new UserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            null,
            false,
            true,
            new List<string> { "Customer" }
        );

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(true);

        _userRepositoryMock
            .Setup(x => x.GetUserRolesAsync(user.Id))
            .ReturnsAsync(new List<string> { "Customer" });

        _tokenServiceMock
            .Setup(x => x.GenerateAccessToken(user, It.IsAny<List<string>>()))
            .Returns(accessToken);

        _tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns(refreshTokenValue);

        _refreshTokenRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<RefreshToken>()))
            .ReturnsAsync(refreshToken);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.FromResult(user));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().Be(refreshTokenValue);
        result.User.Email.Should().Be(user.Email);

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(command.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(command.Password, user.PasswordHash), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentUser_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var command = new LoginCommand("nonexistent@example.com", "password");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(command.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "WrongPassword");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            PasswordHash = "hashed_password"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(false);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");

        _passwordHasherMock.Verify(x => x.VerifyPassword(command.Password, user.PasswordHash), Times.Once);
        _tokenServiceMock.Verify(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InactiveUser_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var command = new LoginCommand("inactive@example.com", "Password123!");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            PasswordHash = "hashed_password",
            IsActive = false
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(true);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*");

        _userRepositoryMock.Setup(x => x.GetUserRolesAsync(It.IsAny<Guid>())).ReturnsAsync(new List<string>());
    }

    [Fact]
    public async Task Handle_ValidLogin_ShouldUpdateLastLoginAt()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Pass123!");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            PasswordHash = "hashed",
            IsActive = true,
            LastLoginAt = null
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword(command.Password, user.PasswordHash)).Returns(true);
        _userRepositoryMock.Setup(x => x.GetUserRolesAsync(user.Id)).ReturnsAsync(new List<string> { "Customer" });
        _tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>())).Returns("token");
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh");
        _refreshTokenRepositoryMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>())).ReturnsAsync(new RefreshToken());
        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(user));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(u =>
            u.LastLoginAt != null && u.LastLoginAt.Value > DateTime.UtcNow.AddMinutes(-1)
        )), Times.Once);
    }
}
