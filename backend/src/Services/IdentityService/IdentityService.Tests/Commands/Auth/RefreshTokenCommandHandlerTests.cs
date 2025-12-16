using FluentAssertions;
using Moq;
using IdentityService.Application.DTOs;
using IdentityService.Application.Features.Auth.Commands.RefreshToken;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;

namespace IdentityService.Tests.Commands.Auth;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();

        _handler = new RefreshTokenCommandHandler(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _tokenServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidRefreshToken_ShouldReturnNewTokens()
    {
        // Arrange
        var command = new RefreshTokenCommand("valid_refresh_token");
        var userId = Guid.NewGuid();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = command.RefreshToken,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };

        var newAccessToken = "new_access_token";
        var newRefreshTokenValue = "new_refresh_token";
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshTokenValue,
            UserId = userId,
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

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(command.RefreshToken))
            .ReturnsAsync(refreshToken);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.GetUserRolesAsync(userId))
            .ReturnsAsync(new List<string> { "Customer" });

        _tokenServiceMock
            .Setup(x => x.GenerateAccessToken(user, It.IsAny<List<string>>()))
            .Returns(newAccessToken);

        _tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns(newRefreshTokenValue);

        _refreshTokenRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.FromResult(refreshToken));

        _refreshTokenRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<RefreshToken>()))
            .ReturnsAsync(newRefreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(newAccessToken);
        result.RefreshToken.Should().Be(newRefreshTokenValue);
        result.User.Email.Should().Be(user.Email);

        _refreshTokenRepositoryMock.Verify(x => x.GetByTokenAsync(command.RefreshToken), Times.Once);
        _refreshTokenRepositoryMock.Verify(x => x.UpdateAsync(It.Is<RefreshToken>(rt => rt.IsRevoked)), Times.Once);
        _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRefreshToken_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var command = new RefreshTokenCommand("invalid_token");

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(command.RefreshToken))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid or expired refresh token");

        _refreshTokenRepositoryMock.Verify(x => x.GetByTokenAsync(command.RefreshToken), Times.Once);
        _tokenServiceMock.Verify(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ExpiredRefreshToken_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var command = new RefreshTokenCommand("expired_token");

        var expiredToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = command.RefreshToken,
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            IsRevoked = false
        };

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(command.RefreshToken))
            .ReturnsAsync(expiredToken);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid or expired refresh token");

        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RevokedRefreshToken_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var command = new RefreshTokenCommand("revoked_token");

        var revokedToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = command.RefreshToken,
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = true,
            RevokedAt = DateTime.UtcNow.AddHours(-1)
        };

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(command.RefreshToken))
            .ReturnsAsync(revokedToken);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid or expired refresh token");

        _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InactiveUser_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var command = new RefreshTokenCommand("valid_token");
        var userId = Guid.NewGuid();

        var refreshToken = new RefreshToken
        {
            Token = command.RefreshToken,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        var inactiveUser = new User
        {
            Id = userId,
            Email = "inactive@example.com",
            IsActive = false
        };

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(command.RefreshToken))
            .ReturnsAsync(refreshToken);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(inactiveUser);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User not found or inactive");

        _tokenServiceMock.Verify(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidToken_ShouldRevokeOldToken()
    {
        // Arrange
        var command = new RefreshTokenCommand("token_to_revoke");
        var userId = Guid.NewGuid();
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = command.RefreshToken,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        var user = new User { Id = userId, Email = "test@example.com", FirstName = "Test", LastName = "User", IsActive = true };

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(command.RefreshToken)).ReturnsAsync(refreshToken);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetUserRolesAsync(userId)).ReturnsAsync(new List<string> { "Customer" });
        _tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>())).Returns("new_token");
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("new_refresh");
        _refreshTokenRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<RefreshToken>())).Returns(Task.FromResult(refreshToken));
        _refreshTokenRepositoryMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>())).ReturnsAsync(new RefreshToken());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _refreshTokenRepositoryMock.Verify(x => x.UpdateAsync(It.Is<RefreshToken>(rt =>
            rt.IsRevoked && rt.RevokedAt != null
        )), Times.Once);
    }
}
