using AutoMapper;
using FluentAssertions;
using Moq;
using IdentityService.Application.DTOs;
using IdentityService.Application.Features.Auth.Commands.Register;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;

namespace IdentityService.Tests.Commands.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _mapperMock = new Mock<IMapper>();

        _handler = new RegisterCommandHandler(
            _userRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldRegisterUser()
    {
        // Arrange
        var command = new RegisterCommand(
            "test@example.com",
            "SecurePass123!",
            "John",
            "Doe",
            null
        );

        var hashedPassword = "hashed_password_123";
        var customerRole = new Role { Id = Guid.NewGuid(), Name = "Customer" };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            PasswordHash = hashedPassword,
            FirstName = command.FirstName,
            LastName = command.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
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

        var authResponse = new AuthResponseDto(
            accessToken,
            refreshTokenValue,
            DateTime.UtcNow.AddHours(1),
            new UserDto(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                null,
                false,
                true,
                new List<string> { "Customer" }
            )
        );

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(x => x.HashPassword(command.Password))
            .Returns(hashedPassword);

        _roleRepositoryMock
            .Setup(x => x.GetByNameAsync("Customer"))
            .ReturnsAsync(customerRole);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(user);

        _roleRepositoryMock
            .Setup(x => x.AddUserToRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

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

        _mapperMock
            .Setup(x => x.Map<UserDto>(user))
            .Returns(authResponse.User);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().Be(refreshTokenValue);
        result.User.Email.Should().Be(command.Email);
        result.User.FirstName.Should().Be(command.FirstName);
        result.User.LastName.Should().Be(command.LastName);

        _userRepositoryMock.Verify(x => x.ExistsByEmailAsync(command.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.HashPassword(command.Password), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new RegisterCommand(
            "existing@example.com",
            "SecurePass123!",
            "Jane",
            "Doe",
            null
        );

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email
        };

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already exists");

        _userRepositoryMock.Verify(x => x.ExistsByEmailAsync(command.Email), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldHashPassword()
    {
        // Arrange
        var command = new RegisterCommand(
            "test@example.com",
            "PlainPassword123!",
            "John",
            "Doe",
            null
        );

        var hashedPassword = "hashed_secure_password";
        var customerRole = new Role { Id = Guid.NewGuid(), Name = "Customer" };
        var user = new User { Id = Guid.NewGuid(), Email = command.Email };

        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(command.Email)).ReturnsAsync(false);
        _passwordHasherMock.Setup(x => x.HashPassword(command.Password)).Returns(hashedPassword);
        _roleRepositoryMock.Setup(x => x.GetByNameAsync("Customer")).ReturnsAsync(customerRole);
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync(user);
        _roleRepositoryMock.Setup(x => x.AddUserToRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(x => x.GetUserRolesAsync(user.Id)).ReturnsAsync(new List<string> { "Customer" });
        _tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>())).Returns("token");
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh");
        _refreshTokenRepositoryMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>())).ReturnsAsync(new RefreshToken());
        _mapperMock.Setup(x => x.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto(Guid.NewGuid(), "test@example.com", "John", "Doe", null, false, true, new List<string> { "Customer" }));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordHasherMock.Verify(x => x.HashPassword("PlainPassword123!"), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldAssignCustomerRole()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Pass123!", "John", "Doe", null);
        var customerRole = new Role { Id = Guid.NewGuid(), Name = "Customer" };
        var user = new User { Id = Guid.NewGuid() };

        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed");
        _roleRepositoryMock.Setup(x => x.GetByNameAsync("Customer")).ReturnsAsync(customerRole);
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync(user);
        _roleRepositoryMock.Setup(x => x.AddUserToRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(x => x.GetUserRolesAsync(user.Id)).ReturnsAsync(new List<string> { "Customer" });
        _tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>())).Returns("token");
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh");
        _refreshTokenRepositoryMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>())).ReturnsAsync(new RefreshToken());
        _mapperMock.Setup(x => x.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto(Guid.NewGuid(), "test@example.com", "John", "Doe", null, false, true, new List<string> { "Customer" }));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _roleRepositoryMock.Verify(x => x.GetByNameAsync("Customer"), Times.Once);
    }
}
