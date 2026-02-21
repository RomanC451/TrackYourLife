using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateProSubscriptionPeriodEnd;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.UpdateProSubscriptionPeriodEnd;

public sealed class UpdateProSubscriptionPeriodEndCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly UpdateProSubscriptionPeriodEndCommandHandler _handler;

    public UpdateProSubscriptionPeriodEndCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new UpdateProSubscriptionPeriodEndCommandHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndIsPro_UpdatesPeriodEndAndRepository()
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
        var newPeriodEnd = DateTime.UtcNow.AddMonths(2);
        var command = new UpdateProSubscriptionPeriodEndCommand(
            userId,
            newPeriodEnd,
            SubscriptionStatus.Active,
            true
        );

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.SubscriptionEndsAtUtc.Should().Be(newPeriodEnd);
        user.SubscriptionStatus.Should().Be(SubscriptionStatus.Active);
        user.SubscriptionCancelAtPeriodEnd.Should().BeTrue();
        _userRepository.Received(1).Update(user);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var command = new UpdateProSubscriptionPeriodEndCommand(
            userId,
            DateTime.UtcNow.AddMonths(1),
            SubscriptionStatus.Active,
            false
        );

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound(userId));
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_WhenUserIsNotPro_ReturnsNotProError()
    {
        var userId = UserId.NewId();
        var email = Email.Create("test@example.com").Value;
        var firstName = Name.Create("John").Value;
        var lastName = Name.Create("Doe").Value;
        var password = new HashedPassword("hashedPassword123");
        var user = User.Create(userId, email, password, firstName, lastName).Value;
        var command = new UpdateProSubscriptionPeriodEndCommand(
            userId,
            DateTime.UtcNow.AddMonths(1),
            SubscriptionStatus.Active,
            false
        );

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.Subscription.NotPro);
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
    }
}
