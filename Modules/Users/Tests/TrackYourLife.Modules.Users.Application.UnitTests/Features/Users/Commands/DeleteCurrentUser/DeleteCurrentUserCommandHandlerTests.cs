using TrackYourLife.Modules.Users.Application.Features.Users.Commands.DeleteCurrentUser;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.DeleteCurrentUser;

public sealed class RemoveCurrentUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly RemoveCurrentUserCommandHandler _handler;

    public RemoveCurrentUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new RemoveCurrentUserCommandHandler(_userRepository, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenUserExists_DeletesUserAndReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var email = Email.Create("test@example.com").Value;
        var firstName = Name.Create("John").Value;
        var lastName = Name.Create("Doe").Value;
        var password = new HashedPassword("hashedPassword123");
        var user = User.Create(userId, email, password, firstName, lastName).Value;
        var command = new RemoveCurrentUserCommand();

        _userIdentifierProvider.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepository.Received(1).Remove(user);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new RemoveCurrentUserCommand();

        _userIdentifierProvider.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound(userId));
        _userRepository.DidNotReceive().Remove(Arg.Any<User>());
    }
}
