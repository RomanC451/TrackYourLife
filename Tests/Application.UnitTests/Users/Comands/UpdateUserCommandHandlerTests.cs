using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Users.Commands.Update;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.ValueObjects;
using TrackYourLifeDotnet.Domain.Entities;
using Xunit;

namespace TrackYourLifeDotnet.Application.UnitTests.Users.Comands;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly UpdateUserCommandHandler _sut;

    private const string jwtTokenString = "validJwtToken";

    public UpdateUserCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _authServiceMock = new Mock<IAuthService>();

        _sut = new UpdateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _authServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccessResult()
    {
        // Arrange
        var command = new UpdateUserCommand(jwtTokenString, "John", "Doe");

        var user = User.Create(
            Guid.NewGuid(),
            Email.Create("johndoe@example.com").Value,
            new HashedPassword("password"),
            Name.Create("John").Value,
            Name.Create("Doe").Value
        );

        _authServiceMock.Setup(x => x.GetUserIdFromJwtToken()).Returns(Result.Success(user.Id));

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
        Assert.Equal(user.Email.Value, result.Value.Email);
        Assert.Equal(command.FirstName, result.Value.FirstName);
        Assert.Equal(command.LastName, result.Value.LastName);

        _authServiceMock.Verify(x => x.GetUserIdFromJwtToken(), Times.Once);
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidJwtToken_ReturnsFailureResult()
    {
        // Arrange
        var command = new UpdateUserCommand(jwtTokenString, "John", "Doe");

        _authServiceMock
            .Setup(x => x.GetUserIdFromJwtToken())
            .Returns(Result.Failure<Guid>(DomainErrors.JwtToken.Invalid));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.JwtToken.Invalid, result.Error);

        _authServiceMock.Verify(x => x.GetUserIdFromJwtToken(), Times.Once);
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonexistentUser_ReturnsFailureResult()
    {
        // Arrange
        var command = new UpdateUserCommand(jwtTokenString, "John", "Doe");
        var userId = Guid.NewGuid();
        _authServiceMock.Setup(x => x.GetUserIdFromJwtToken()).Returns(Result.Success(userId));

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.User.NotFound(userId), result.Error);

        _authServiceMock.Verify(x => x.GetUserIdFromJwtToken(), Times.Once);
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidFirstName_ReturnsFailureResult()
    {
        // Arrange
        var command = new UpdateUserCommand(jwtTokenString, "", "Doe");

        var userId = Guid.NewGuid();
        var user = User.Create(
            Guid.NewGuid(),
            Email.Create("johndoe@example.com").Value,
            new HashedPassword("password"),
            Name.Create("John").Value,
            Name.Create("Doe").Value
        );

        _authServiceMock.Setup(x => x.GetUserIdFromJwtToken()).Returns(Result.Success(userId));

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, CancellationToken.None))
            .ReturnsAsync(user);

        var handler = new UpdateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _authServiceMock.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Name.Empty, result.Error);

        _authServiceMock.Verify(x => x.GetUserIdFromJwtToken(), Times.Once);
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidLastName_ReturnsFailureResult()
    {
        // Arrange
        var command = new UpdateUserCommand(jwtTokenString, "John", "");

        var userId = Guid.NewGuid();
        var user = User.Create(
            Guid.NewGuid(),
            Email.Create("johndoe@example.com").Value,
            new HashedPassword("password"),
            Name.Create("John").Value,
            Name.Create("Doe").Value
        );

        _authServiceMock.Setup(x => x.GetUserIdFromJwtToken()).Returns(Result.Success(userId));

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, CancellationToken.None))
            .ReturnsAsync(user);

        var handler = new UpdateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _authServiceMock.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Name.Empty, result.Error);

        _authServiceMock.Verify(x => x.GetUserIdFromJwtToken(), Times.Once);
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
