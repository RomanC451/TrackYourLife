using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpgradeToPro;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.UpgradeToPro;

public sealed class UpgradeToProCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly UpgradeToProCommandHandler _handler;

    public UpgradeToProCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new UpgradeToProCommandHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_WhenUserExists_SetsProSubscriptionAndUpdatesRepository()
    {
        var userId = UserId.NewId();
        var email = Email.Create("test@example.com").Value;
        var firstName = Name.Create("John").Value;
        var lastName = Name.Create("Doe").Value;
        var password = new HashedPassword("hashedPassword123");
        var user = User.Create(userId, email, password, firstName, lastName).Value;
        var periodEnd = DateTime.UtcNow.AddMonths(1);
        var command = new UpgradeToProCommand(userId, "cus_Stripe123", periodEnd);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.PlanType.Should().Be(PlanType.Pro);
        user.StripeCustomerId.Should().Be("cus_Stripe123");
        user.SubscriptionEndsAtUtc.Should().Be(periodEnd);
        _userRepository.Received(1).Update(user);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var command = new UpgradeToProCommand(
            userId,
            "cus_Stripe123",
            DateTime.UtcNow.AddMonths(1)
        );

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound(userId));
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
    }
}
