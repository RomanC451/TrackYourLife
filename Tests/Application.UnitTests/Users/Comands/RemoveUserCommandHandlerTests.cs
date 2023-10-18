using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Users.Commands.Remove;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.ValueObjects;
using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Application.UnitTests.Users.Comands;

public class RemoveUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IAuthService> _authServiceMock = new();
    private readonly RemoveUserCommandHandler _sut;

    private const string validJwtToken = "validJwtToken";

    public RemoveUserCommandHandlerTests()
    {
        _sut = new RemoveUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _authServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenUserFound_RemovesUserAndReturnsSuccessResult()
    {
        // Arrange
        var command = new RemoveUserCommand(validJwtToken);
        var user = User.Create(
            Guid.NewGuid(),
            Email.Create("johndoe@example.com").Value,
            new HashedPassword("password"),
            Name.Create("John").Value,
            Name.Create("Doe").Value
        );
        _authServiceMock
            .Setup(x => x.GetUserIdFromJwtToken(command.JwtToken))
            .Returns(Result.Success(user.Id));

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.UserId);
        _userRepositoryMock.Verify(x => x.Remove(user), Times.Once);
        _authServiceMock.Verify(x => x.GetUserIdFromJwtToken(command.JwtToken), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenJwtTokenIsInvalid_ReturnsFailureResult()
    {
        // Arrange
        var command = new RemoveUserCommand(validJwtToken);
        _authServiceMock
            .Setup(x => x.GetUserIdFromJwtToken(command.JwtToken))
            .Returns(Result.Failure<Guid>(DomainErrors.JwtToken.Invalid));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.JwtToken.Invalid, result.Error);

        _userRepositoryMock.Verify(x => x.Remove(It.IsAny<User>()), Times.Never);
        _authServiceMock.Verify(x => x.GetUserIdFromJwtToken(command.JwtToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailureResult()
    {
        // Arrange
        var command = new RemoveUserCommand(validJwtToken);
        var userId = Guid.NewGuid();
        _authServiceMock
            .Setup(x => x.GetUserIdFromJwtToken(command.JwtToken))
            .Returns(Result.Success(userId));
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.User.NotFound(userId), result.Error);

        _userRepositoryMock.Verify(x => x.Remove(It.IsAny<User>()), Times.Never);
        _authServiceMock.Verify(x => x.GetUserIdFromJwtToken(command.JwtToken), Times.Once);
    }
}
