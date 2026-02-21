using MassTransit;
using MassTransit.Testing;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Features.Checkout.CreateCheckoutSession;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.Checkout.CreateCheckoutSession;

public sealed class CreateCheckoutSessionCommandHandlerTests : IAsyncLifetime
{
    private InMemoryTestHarness _harness = null!;
    private IBus _bus = null!;
    private readonly IStripeService _stripeService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private CreateCheckoutSessionCommandHandler _handler = null!;
    private GetUserForBillingByIdResponse? _userResponseToReturn;

    public CreateCheckoutSessionCommandHandlerTests()
    {
        _stripeService = Substitute.For<IStripeService>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
    }

    public async Task InitializeAsync()
    {
        _harness = new InMemoryTestHarness();
        _userResponseToReturn = new GetUserForBillingByIdResponse(
            new UserForBillingDto(UserId.NewId(), "john@example.com", null, false),
            []
        );
        _harness.OnConfigureBus += configurator =>
        {
            configurator.ReceiveEndpoint(
                "get-user-for-billing",
                e =>
                {
                    e.Handler<GetUserForBillingByIdRequest>(async context =>
                    {
                        var response =
                            _userResponseToReturn ?? new GetUserForBillingByIdResponse(null, []);
                        await context.RespondAsync(response);
                    });
                }
            );
        };
        await _harness.Start();
        _bus = _harness.Bus;
        _handler = new CreateCheckoutSessionCommandHandler(
            _stripeService,
            _userIdentifierProvider,
            _bus
        );
    }

    public async Task DisposeAsync() => await _harness.Stop();

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsCheckoutUrl()
    {
        var userId = UserId.NewId();
        _userResponseToReturn = new GetUserForBillingByIdResponse(
            new UserForBillingDto(userId, "john@example.com", null, false),
            []
        );
        var command = new CreateCheckoutSessionCommand(
            "https://app/success",
            "https://app/cancel",
            "price_123"
        );
        const string expectedUrl = "https://checkout.stripe.com/session_abc";

        _userIdentifierProvider.UserId.Returns(userId);
        _stripeService
            .CreateCheckoutSessionAsync(
                null,
                "john@example.com",
                userId.Value.ToString(),
                "price_123",
                "https://app/success",
                "https://app/cancel",
                Arg.Any<CancellationToken>()
            )
            .Returns(expectedUrl);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedUrl);
        await _stripeService
            .Received(1)
            .CreateCheckoutSessionAsync(
                null,
                "john@example.com",
                userId.Value.ToString(),
                "price_123",
                "https://app/success",
                "https://app/cancel",
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsFailure()
    {
        var userId = UserId.NewId();
        _userResponseToReturn = new GetUserForBillingByIdResponse(
            null,
            [new SharedLib.Domain.Errors.Error("User.NotFound", "User not found.", 404)]
        );
        var command = new CreateCheckoutSessionCommand(
            "https://app/success",
            "https://app/cancel",
            "price_123"
        );

        _userIdentifierProvider.UserId.Returns(userId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        await _stripeService
            .DidNotReceive()
            .CreateCheckoutSessionAsync(
                Arg.Any<string?>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenUserHasStripeCustomerId_PassesItToStripeService()
    {
        var userId = UserId.NewId();
        _userResponseToReturn = new GetUserForBillingByIdResponse(
            new UserForBillingDto(userId, "jane@example.com", "cus_Existing123", false),
            []
        );
        var command = new CreateCheckoutSessionCommand(
            "https://app/success",
            "https://app/cancel",
            "price_456"
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _stripeService
            .CreateCheckoutSessionAsync(
                Arg.Any<string?>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            )
            .Returns("https://checkout.stripe.com/session_xyz");

        await _handler.Handle(command, CancellationToken.None);

        await _stripeService
            .Received(1)
            .CreateCheckoutSessionAsync(
                "cus_Existing123",
                "jane@example.com",
                userId.Value.ToString(),
                "price_456",
                "https://app/success",
                "https://app/cancel",
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenUserHasActiveProSubscription_ReturnsFailure()
    {
        var userId = UserId.NewId();
        _userResponseToReturn = new GetUserForBillingByIdResponse(
            new UserForBillingDto(userId, "pro@example.com", "cus_Pro123", true),
            []
        );
        var command = new CreateCheckoutSessionCommand(
            "https://app/success",
            "https://app/cancel",
            "price_123"
        );

        _userIdentifierProvider.UserId.Returns(userId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Checkout.AlreadySubscribed");
        result.Error.Message.Should().Be("You already have an active Pro subscription.");
        await _stripeService
            .DidNotReceive()
            .CreateCheckoutSessionAsync(
                Arg.Any<string?>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenStripeReportsActiveSubscriptionForPrice_ReturnsFailure()
    {
        var userId = UserId.NewId();
        _userResponseToReturn = new GetUserForBillingByIdResponse(
            new UserForBillingDto(userId, "user@example.com", "cus_Stripe123", false),
            []
        );
        _stripeService
            .CustomerHasActiveSubscriptionForPriceAsync(
                "cus_Stripe123",
                "price_123",
                Arg.Any<CancellationToken>()
            )
            .Returns(true);
        var command = new CreateCheckoutSessionCommand(
            "https://app/success",
            "https://app/cancel",
            "price_123"
        );

        _userIdentifierProvider.UserId.Returns(userId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Checkout.AlreadySubscribed");
        result.Error.Message.Should().Be("You already have an active subscription for this plan.");
        await _stripeService
            .DidNotReceive()
            .CreateCheckoutSessionAsync(
                Arg.Any<string?>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }
}
