using MassTransit;
using MassTransit.Testing;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionDeleted;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.Webhooks.HandleSubscriptionDeleted;

public sealed class HandleSubscriptionDeletedCommandHandlerTests : IAsyncLifetime
{
    private InMemoryTestHarness _harness = null!;
    private IBus _bus = null!;
    private HandleSubscriptionDeletedCommandHandler _handler = null!;
    private GetUserForBillingByStripeCustomerIdResponse? _userResponseToReturn;
    private DowngradeProResponse? _downgradeResponseToReturn;

    public async Task InitializeAsync()
    {
        _harness = new InMemoryTestHarness();
        var userId = UserId.NewId();
        _userResponseToReturn = new GetUserForBillingByStripeCustomerIdResponse(
            new UserForBillingByStripeCustomerIdDto(userId, "user@example.com"),
            []
        );
        _downgradeResponseToReturn = new DowngradeProResponse([]);
        _harness.OnConfigureBus += configurator =>
        {
            configurator.ReceiveEndpoint(
                "get-user-for-billing-stripe",
                e =>
                {
                    e.Handler<GetUserForBillingByStripeCustomerIdRequest>(async context =>
                    {
                        var response = _userResponseToReturn
                            ?? new GetUserForBillingByStripeCustomerIdResponse(null, []);
                        await context.RespondAsync(response);
                    });
                }
            );
            configurator.ReceiveEndpoint(
                "downgrade-pro",
                e =>
                {
                    e.Handler<DowngradeProRequest>(async context =>
                    {
                        var response = _downgradeResponseToReturn ?? new DowngradeProResponse([]);
                        await context.RespondAsync(response);
                    });
                }
            );
        };
        await _harness.Start();
        _bus = _harness.Bus;
        _handler = new HandleSubscriptionDeletedCommandHandler(_bus);
    }

    public async Task DisposeAsync() => await _harness.Stop();

    [Fact]
    public async Task Handle_WhenCustomerIdEmpty_ReturnsFailure()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.deleted",
            null,
            "",
            "sub_1",
            null,
            "canceled",
            false
        );
        var command = new HandleSubscriptionDeletedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("MissingCustomerId");
    }

    [Fact]
    public async Task Handle_WhenSubscriptionStatusInvalid_ReturnsFailure()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.deleted",
            null,
            "cus_1",
            "sub_1",
            null,
            "invalid_status",
            false
        );
        var command = new HandleSubscriptionDeletedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("InvalidSubscriptionStatus");
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailure()
    {
        _userResponseToReturn = new GetUserForBillingByStripeCustomerIdResponse(
            null,
            [new SharedLib.Domain.Errors.Error("User.NotFound", "Not found.", 404)]
        );
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.deleted",
            null,
            "cus_unknown",
            "sub_1",
            null,
            "canceled",
            false
        );
        var command = new HandleSubscriptionDeletedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenUserDataNull_ReturnsFailure()
    {
        _userResponseToReturn = new GetUserForBillingByStripeCustomerIdResponse(null, []);
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.deleted",
            null,
            "cus_1",
            "sub_1",
            null,
            "canceled",
            false
        );
        var command = new HandleSubscriptionDeletedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("UserNotFound");
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsSuccess()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.deleted",
            null,
            "cus_1",
            "sub_1",
            null,
            "canceled",
            false
        );
        var command = new HandleSubscriptionDeletedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenDowngradeReturnsErrors_ReturnsFailure()
    {
        _downgradeResponseToReturn = new DowngradeProResponse(
            [new SharedLib.Domain.Errors.Error("Downgrade.Failed", "Failed.", 500)]
        );
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.deleted",
            null,
            "cus_1",
            "sub_1",
            null,
            "canceled",
            false
        );
        var command = new HandleSubscriptionDeletedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}
