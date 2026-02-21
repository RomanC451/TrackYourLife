using MassTransit;
using MassTransit.Testing;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Features.BillingPortal.GetBillingPortalUrl;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.BillingPortal.GetBillingPortalUrl;

public sealed class GetBillingPortalUrlQueryHandlerTests : IAsyncLifetime
{
    private InMemoryTestHarness _harness = null!;
    private IBus _bus = null!;
    private readonly IStripeService _stripeService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private GetBillingPortalUrlQueryHandler _handler = null!;
    private GetUserForBillingByIdResponse? _userResponseToReturn;

    public GetBillingPortalUrlQueryHandlerTests()
    {
        _stripeService = Substitute.For<IStripeService>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
    }

    public async Task InitializeAsync()
    {
        _harness = new InMemoryTestHarness();
        _userResponseToReturn = new GetUserForBillingByIdResponse(
            new UserForBillingDto(UserId.NewId(), "john@example.com", "cus_123", false),
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
        _handler = new GetBillingPortalUrlQueryHandler(
            _stripeService,
            _userIdentifierProvider,
            _bus
        );
    }

    public async Task DisposeAsync() => await _harness.Stop();

    [Fact]
    public async Task Handle_WhenUserExistsWithStripeCustomerId_ReturnsPortalUrl()
    {
        var userId = UserId.NewId();
        _userResponseToReturn = new GetUserForBillingByIdResponse(
            new UserForBillingDto(userId, "john@example.com", "cus_abc", false),
            []
        );
        var query = new GetBillingPortalUrlQuery("https://app/billing");
        const string expectedUrl = "https://billing.stripe.com/session/xyz";

        _userIdentifierProvider.UserId.Returns(userId);
        _stripeService
            .CreateBillingPortalSessionAsync(
                "cus_abc",
                "https://app/billing",
                Arg.Any<CancellationToken>()
            )
            .Returns(expectedUrl);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedUrl);
        await _stripeService
            .Received(1)
            .CreateBillingPortalSessionAsync(
                "cus_abc",
                "https://app/billing",
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
        var query = new GetBillingPortalUrlQuery("https://app/billing");

        _userIdentifierProvider.UserId.Returns(userId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        await _stripeService
            .DidNotReceive()
            .CreateBillingPortalSessionAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenUserIsNull_ReturnsUserNotFound()
    {
        var userId = UserId.NewId();
        _userResponseToReturn = new GetUserForBillingByIdResponse(null, []);
        var query = new GetBillingPortalUrlQuery("https://app/billing");

        _userIdentifierProvider.UserId.Returns(userId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("BillingPortal.UserNotFound");
        result.Error.Message.Should().Be("User not found.");
        await _stripeService
            .DidNotReceive()
            .CreateBillingPortalSessionAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenUserHasNoStripeCustomerId_ReturnsNoStripeCustomer()
    {
        var userId = UserId.NewId();
        _userResponseToReturn = new GetUserForBillingByIdResponse(
            new UserForBillingDto(userId, "jane@example.com", null, false),
            []
        );
        var query = new GetBillingPortalUrlQuery("https://app/billing");

        _userIdentifierProvider.UserId.Returns(userId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("BillingPortal.NoStripeCustomer");
        result.Error.Message.Should().Be("No Stripe customer linked to this account.");
        await _stripeService
            .DidNotReceive()
            .CreateBillingPortalSessionAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenUserHasEmptyStripeCustomerId_ReturnsNoStripeCustomer()
    {
        var userId = UserId.NewId();
        _userResponseToReturn = new GetUserForBillingByIdResponse(
            new UserForBillingDto(userId, "jane@example.com", "", false),
            []
        );
        var query = new GetBillingPortalUrlQuery("https://app/billing");

        _userIdentifierProvider.UserId.Returns(userId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("BillingPortal.NoStripeCustomer");
        await _stripeService
            .DidNotReceive()
            .CreateBillingPortalSessionAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }
}
