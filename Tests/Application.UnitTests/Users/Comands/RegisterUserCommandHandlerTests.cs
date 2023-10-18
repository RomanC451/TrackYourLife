using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Users.Commands.Register;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.ValueObjects;
using TrackYourLifeDotnet.Domain.Entities;
using Microsoft.FeatureManagement;

namespace TrackYourLifeDotnet.Application.UnitTests.Users.Comands;

public class RegisterUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<IFeatureManager> _featureManager = new();
    private readonly RegisterUserCommandHandler _sut;

    public RegisterUserHandlerTests()
    {
        _sut = new RegisterUserCommandHandler(
            _userRepository.Object,
            _unitOfWork.Object,
            _passwordHasher.Object,
            _featureManager.Object
        );
    }

    [Fact]
    public async Task Handler_WithValidCommand_ShouldCreateNewUser()
    {
        // Arrange
        RegisterUserCommand command = new("test@test.com", "Test.1234", "John", "Doe");

        HashedPassword passwordHash = new("HashedPassword");

        _passwordHasher.Setup(hasher => hasher.Hash(command.Password)).Returns(passwordHash);

        Email email = Email.Create(command.Email).Value;

        Name firstName = Name.Create(command.FirstName).Value;
        Name lastName = Name.Create(command.LastName).Value;

        _userRepository
            .Setup(repo => repo.IsEmailUniqueAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()));

        User createdUser = null!;

        _userRepository
            .Setup(repo => repo.Add(It.IsAny<User>()))
            .Callback<User>(user => createdUser = user);

        // Act
        Result<RegisterUserResponse> result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        RegisterUserResponse response = result.Value;

        Assert.NotNull(response);
        Assert.Equal(createdUser.Id, response.UserId);

        _userRepository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
        _userRepository.Verify(
            repo => repo.IsEmailUniqueAsync(email, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _authService.Verify(
            service =>
                service.RefreshUserAuthTokens(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Once);
    }

    [Fact]
    public async Task Handler_WithInvalidEmail_ShouldReturnError()
    {
        // Arrange
        RegisterUserCommand command = new("invalidemail", "Test.1234", "John", "Doe");

        // Act
        Result<RegisterUserResponse> result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(DomainErrors.Email.InvalidFormat, result.Error);

        _userRepository.Verify(
            repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _userRepository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _authService.Verify(
            service =>
                service.RefreshUserAuthTokens(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Never);
    }

    [Fact]
    public async Task Handler_WithInvalidPassword_ShouldReturnError()
    {
        // Arrange
        RegisterUserCommand command = new("test2@test.com", "invalidpassword", "John", "Doe");
        var email = Email.Create(command.Email).Value;
        _userRepository
            .Setup(repo => repo.IsEmailUniqueAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        Result<RegisterUserResponse> result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(DomainErrors.Password.UpperCase, result.Error);

        _userRepository.Verify(
            repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _userRepository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _authService.Verify(
            service =>
                service.RefreshUserAuthTokens(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Never);
    }

    [Fact]
    public async Task Handler_WithExistingEmail_ShouldReturnError()
    {
        // Arrange
        RegisterUserCommand command = new("test@test.com", "Test.1234", "John", "Doe");

        _userRepository
            .Setup(
                repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(false);

        // Act
        Result<RegisterUserResponse> result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(DomainErrors.Email.AlreadyUsed, result.Error);

        _userRepository.Verify(
            repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _userRepository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _authService.Verify(
            service =>
                service.RefreshUserAuthTokens(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Never);
    }

    [Fact]
    public async Task Handler_WithInvalidLastName_ShouldReturnError()
    {
        // Arrange
        RegisterUserCommand command = new("test@test.com", "Test.1234", "John", "");

        _userRepository
            .Setup(
                repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        // Act
        Result<RegisterUserResponse> result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(DomainErrors.Name.Empty, result.Error);

        _userRepository.Verify(
            repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _userRepository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _authService.Verify(
            service =>
                service.RefreshUserAuthTokens(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Never);
    }

    [Fact]
    public async Task Handler_WithInvalidFirstName_ShouldReturnError()
    {
        // Arrange
        RegisterUserCommand command = new("test@test.com", "Test.1234", "", "Doe");

        _userRepository
            .Setup(
                repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        // Act
        Result<RegisterUserResponse> result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(DomainErrors.Name.Empty, result.Error);

        _userRepository.Verify(
            repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _userRepository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _authService.Verify(
            service =>
                service.RefreshUserAuthTokens(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Never);
    }
}
