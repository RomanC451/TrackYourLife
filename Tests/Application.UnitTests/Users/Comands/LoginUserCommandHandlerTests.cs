using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Users.Commands.Login;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.ValueObjects;
using Xunit;

namespace TrackYourLifeDotnet.Application.UnitTests.Users.Comands;

public class LoginUserHandlerTests
{
    public readonly Mock<IUserRepository> _mockUserRepository = new();
    public readonly Mock<IPasswordHasher> _mockPasswordHasher = new();
    public readonly Mock<IAuthService> _mockAuthService = new();
    public readonly LoginUserCommandHandler _sut;

    public LoginUserHandlerTests()
    {
        _sut = new LoginUserCommandHandler(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockAuthService.Object
        );
    }

    [Fact]
    public async Task Handler_ValidCredentials_ReturnsLoginUserResponse()
    {
        // Arrange
        const string emailValue = "test@test.com";
        const string passwordValue = "Password.001";

        var user = User.Create(
            Guid.NewGuid(),
            Email.Create(emailValue).Value,
            new HashedPassword(passwordValue),
            Name.Create("John").Value,
            Name.Create("Doe").Value
        );

        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(user.Email, default))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(hasher => hasher.Verify(user.Password.Value, passwordValue))
            .Returns(true);

        _mockAuthService
            .Setup(auth => auth.RefreshUserAuthTokens(user, default))
            .ReturnsAsync(("jwtToken", new RefreshToken(Guid.NewGuid(), "refreshToken", user.Id)));

        var command = new LoginUserCommand(emailValue, passwordValue);

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.IsType<LoginUserResponse>(result.Value);
        Assert.Equal(user.Id, result.Value.UserId);
        Assert.Equal("jwtToken", result.Value.JwtToken);
        Assert.Equal(user.Id, result.Value.RefreshToken.UserId);

        _mockUserRepository.Verify(repo => repo.GetByEmailAsync(user.Email, default), Times.Once);
        _mockPasswordHasher.Verify(
            hasher => hasher.Verify(user.Password.Value, passwordValue),
            Times.Once
        );
        _mockAuthService.Verify(auth => auth.RefreshUserAuthTokens(user, default), Times.Once);
    }

    [Fact]
    public async Task Handler_InvalidCredentials_ReturnsFailureResult()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<Email>(), default))
            .ReturnsAsync((User?)null);

        _mockPasswordHasher
            .Setup(hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var command = new LoginUserCommand("test@test.com", "Password.002");

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.User.InvalidCredentials, result.Error);

        _mockUserRepository.Verify(
            repo => repo.GetByEmailAsync(It.IsAny<Email>(), default),
            Times.Once
        );
        _mockPasswordHasher.Verify(
            hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
        _mockAuthService.Verify(
            auth => auth.RefreshUserAuthTokens(It.IsAny<User>(), default),
            Times.Never
        );
    }

    [Fact]
    public async Task Handler_InvalidEmail_ReturnsFailureResult()
    {
        // Arrange
        var command = new LoginUserCommand("invalidemail", "password");

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.User.InvalidCredentials, result.Error);

        _mockUserRepository.Verify(
            repo => repo.GetByEmailAsync(It.IsAny<Email>(), default),
            Times.Never
        );
        _mockPasswordHasher.Verify(
            hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
        _mockAuthService.Verify(
            auth => auth.RefreshUserAuthTokens(It.IsAny<User>(), default),
            Times.Never
        );
    }

    [Fact]
    public async Task Handler_InvalidPassword_ReturnsFailureResult()
    {
        // Arrange
        var command = new LoginUserCommand("test@test.com", "");

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.User.InvalidCredentials, result.Error);

        _mockUserRepository.Verify(
            repo => repo.GetByEmailAsync(It.IsAny<Email>(), default),
            Times.Never
        );
        _mockPasswordHasher.Verify(
            hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
        _mockAuthService.Verify(
            auth => auth.RefreshUserAuthTokens(It.IsAny<User>(), default),
            Times.Never
        );
    }

    [Fact]
    public async Task Handler_NullUser_ReturnsFailureResult()
    {
        // Arrange

        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<Email>(), default))
            .ReturnsAsync((User?)null);

        var command = new LoginUserCommand("test@test.com", "Password.001");

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.User.InvalidCredentials, result.Error);

        _mockUserRepository.Verify(
            repo => repo.GetByEmailAsync(It.IsAny<Email>(), default),
            Times.Once
        );
        _mockPasswordHasher.Verify(
            hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
        _mockAuthService.Verify(
            auth => auth.RefreshUserAuthTokens(It.IsAny<User>(), default),
            Times.Never
        );
    }

    [Fact]
    public async Task Handler_PasswordHashMismatch_ReturnsFailureResult()
    {
        // Arrange
        const string emailValue = "test2@test.com";
        const string passwordValue = "Password.002";

        var user = User.Create(
            Guid.NewGuid(),
            Email.Create(emailValue).Value,
            new HashedPassword(passwordValue),
            Name.Create("John").Value,
            Name.Create("Doe").Value
        );
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(user.Email, default))
            .ReturnsAsync(user);
        _mockPasswordHasher
            .Setup(hasher => hasher.Verify(user.Password.Value, "Wrongpassword.002"))
            .Returns(false);

        var command = new LoginUserCommand(emailValue, "Wrongpassword.002");

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.User.InvalidCredentials, result.Error);

        _mockUserRepository.Verify(
            repo => repo.GetByEmailAsync(It.IsAny<Email>(), default),
            Times.Once
        );
        _mockPasswordHasher.Verify(
            hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once
        );
        _mockAuthService.Verify(
            auth => auth.RefreshUserAuthTokens(It.IsAny<User>(), default),
            Times.Never
        );
    }
}
