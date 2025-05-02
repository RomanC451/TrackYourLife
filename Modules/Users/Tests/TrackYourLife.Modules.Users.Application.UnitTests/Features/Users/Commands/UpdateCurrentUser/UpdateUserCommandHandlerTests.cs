using NSubstitute;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateCurrentUser;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.UpdateCurrentUser;

public sealed class UpdateUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new UpdateUserCommandHandler(_userRepository, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenUserExists_UpdatesNameAndReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var email = Email.Create("test@example.com").Value;
        var firstName = Name.Create("John").Value;
        var lastName = Name.Create("Doe").Value;
        var password = new HashedPassword("hashedPassword123");
        var user = User.Create(userId, email, password, firstName, lastName).Value;
        var command = new UpdateUserCommand("NewJohn", "NewDoe");

        _userIdentifierProvider.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepository.Received(1).Update(user);
        user.FirstName.Value.Should().Be("NewJohn");
        user.LastName.Value.Should().Be("NewDoe");
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new UpdateUserCommand("NewJohn", "NewDoe");

        _userIdentifierProvider.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound(userId));
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
    }
}
