using TrackYourLife.Modules.Users.Application.Features.Users.Commands.SetStripeCustomerId;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.SetStripeCustomerId;

public sealed class SetStripeCustomerIdCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly SetStripeCustomerIdCommandHandler _handler;

    public SetStripeCustomerIdCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new SetStripeCustomerIdCommandHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_WhenUserExists_SetsStripeCustomerIdAndUpdatesRepository()
    {
        var userId = UserId.NewId();
        var email = Email.Create("test@example.com").Value;
        var firstName = Name.Create("John").Value;
        var lastName = Name.Create("Doe").Value;
        var password = new HashedPassword("hashedPassword123");
        var user = User.Create(userId, email, password, firstName, lastName).Value;
        var command = new SetStripeCustomerIdCommand(userId, "cus_NewCustomer123");

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.StripeCustomerId.Should().Be("cus_NewCustomer123");
        _userRepository.Received(1).Update(user);
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndAlreadyHasStripeCustomerId_OverwritesIt()
    {
        var userId = UserId.NewId();
        var email = Email.Create("test@example.com").Value;
        var firstName = Name.Create("John").Value;
        var lastName = Name.Create("Doe").Value;
        var password = new HashedPassword("hashedPassword123");
        var user = User.Create(userId, email, password, firstName, lastName).Value;
        user.SetStripeCustomerId("cus_Old");
        var command = new SetStripeCustomerIdCommand(userId, "cus_New");

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.StripeCustomerId.Should().Be("cus_New");
        _userRepository.Received(1).Update(user);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var command = new SetStripeCustomerIdCommand(userId, "cus_123");

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound(userId));
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
    }
}
