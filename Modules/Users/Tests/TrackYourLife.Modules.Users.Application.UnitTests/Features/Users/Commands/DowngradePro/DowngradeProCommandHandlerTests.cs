using TrackYourLife.Modules.Users.Application.Features.Users.Commands.DowngradePro;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.DowngradePro;

public sealed class DowngradeProCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly DowngradeProCommandHandler _handler;

    public DowngradeProCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new DowngradeProCommandHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ClearsProSubscriptionAndUpdatesRepository()
    {
        var userId = UserId.NewId();
        var email = Email.Create("test@example.com").Value;
        var firstName = Name.Create("John").Value;
        var lastName = Name.Create("Doe").Value;
        var password = new HashedPassword("hashedPassword123");
        var user = User.Create(userId, email, password, firstName, lastName).Value;
        user.SetStripeCustomerId("cus_123");
        user.SetProSubscription(
            "cus_123",
            DateTime.UtcNow.AddMonths(1),
            SubscriptionStatus.Active,
            false
        );
        var command = new DowngradeProCommand(userId, SubscriptionStatus.Canceled);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.PlanType.Should().Be(PlanType.Free);
        user.SubscriptionStatus.Should().Be(SubscriptionStatus.Canceled);
        user.SubscriptionEndsAtUtc.Should().BeNull();
        user.SubscriptionCancelAtPeriodEnd.Should().BeFalse();
        user.StripeCustomerId.Should().Be("cus_123");
        _userRepository.Received(1).Update(user);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var command = new DowngradeProCommand(userId, SubscriptionStatus.Canceled);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound(userId));
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
    }
}
