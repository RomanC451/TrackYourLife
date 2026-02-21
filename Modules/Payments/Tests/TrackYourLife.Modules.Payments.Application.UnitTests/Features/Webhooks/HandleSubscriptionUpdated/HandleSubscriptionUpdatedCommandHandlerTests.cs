using MassTransit;
using MassTransit.Testing;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionUpdated;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.Webhooks.HandleSubscriptionUpdated;

public sealed class HandleSubscriptionUpdatedCommandHandlerTests : IAsyncLifetime
{
    private InMemoryTestHarness _harness = null!;
    private IBus _bus = null!;
    private HandleSubscriptionUpdatedCommandHandler _handler = null!;
    private GetUserForBillingByStripeCustomerIdResponse? _userResponseToReturn;
    private UpdateProSubscriptionPeriodEndResponse? _updateResponseToReturn;
    private DowngradeProResponse? _downgradeResponseToReturn;

    public async Task InitializeAsync()
    {
        _harness = new InMemoryTestHarness();
        var userId = UserId.NewId();
        _userResponseToReturn = new GetUserForBillingByStripeCustomerIdResponse(
            new UserForBillingByStripeCustomerIdDto(userId, "user@example.com"),
            []
        );
        _updateResponseToReturn = new UpdateProSubscriptionPeriodEndResponse([]);
        _downgradeResponseToReturn = new DowngradeProResponse([]);
        _harness.OnConfigureBus += configurator =>
        {
            configurator.ReceiveEndpoint(
                "get-user-for-billing-stripe-upd",
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
                "update-pro-subscription-period",
                e =>
                {
                    e.Handler<UpdateProSubscriptionPeriodEndRequest>(async context =>
                    {
                        var response = _updateResponseToReturn
                            ?? new UpdateProSubscriptionPeriodEndResponse([]);
                        await context.RespondAsync(response);
                    });
                }
            );
            configurator.ReceiveEndpoint(
                "downgrade-pro-upd",
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
        _handler = new HandleSubscriptionUpdatedCommandHandler(_bus);
    }

    public async Task DisposeAsync() => await _harness.Stop();

    [Fact]
    public void Command_CanBeConstructed()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.updated",
            null,
            "cus_1",
            null,
            null,
            "active",
            false
        );

        var command = new HandleSubscriptionUpdatedCommand(payload);

        command.Payload.Should().Be(payload);
    }

    [Fact]
    public async Task Handle_WhenCustomerIdEmpty_ReturnsFailure()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.updated",
            null,
            "",
            "sub_1",
            DateTime.UtcNow.AddMonths(1),
            "active",
            false
        );
        var command = new HandleSubscriptionUpdatedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("MissingCustomerId");
    }

    [Fact]
    public async Task Handle_WhenSubscriptionStatusInvalid_ReturnsFailure()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.updated",
            null,
            "cus_1",
            "sub_1",
            DateTime.UtcNow.AddMonths(1),
            "invalid_status",
            false
        );
        var command = new HandleSubscriptionUpdatedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("InvalidSubscriptionStatus");
    }

    [Fact]
    public async Task Handle_WhenUserDataNull_ReturnsFailure()
    {
        _userResponseToReturn = new GetUserForBillingByStripeCustomerIdResponse(null, []);
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.updated",
            null,
            "cus_1",
            "sub_1",
            DateTime.UtcNow.AddMonths(1),
            "active",
            false
        );
        var command = new HandleSubscriptionUpdatedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("UserNotFound");
    }

    [Fact]
    public async Task Handle_WhenActiveWithPeriodEnd_CallsUpdatePeriodEnd_ReturnsSuccess()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.updated",
            null,
            "cus_1",
            "sub_1",
            DateTime.UtcNow.AddMonths(1),
            "active",
            true
        );
        var command = new HandleSubscriptionUpdatedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenCanceled_CallsDowngrade_ReturnsSuccess()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.updated",
            null,
            "cus_1",
            "sub_1",
            null,
            "canceled",
            false
        );
        var command = new HandleSubscriptionUpdatedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenPastDue_CallsDowngrade_ReturnsSuccess()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.updated",
            null,
            "cus_1",
            "sub_1",
            null,
            "past_due",
            false
        );
        var command = new HandleSubscriptionUpdatedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenUnpaid_CallsDowngrade_ReturnsSuccess()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.updated",
            null,
            "cus_1",
            "sub_1",
            null,
            "unpaid",
            false
        );
        var command = new HandleSubscriptionUpdatedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenActiveNoPeriodEnd_ReturnsSuccess()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.updated",
            null,
            "cus_1",
            "sub_1",
            null,
            "active",
            false
        );
        var command = new HandleSubscriptionUpdatedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenUpdatePeriodEndReturnsErrors_ReturnsFailure()
    {
        _updateResponseToReturn = new UpdateProSubscriptionPeriodEndResponse(
            [new SharedLib.Domain.Errors.Error("Update.Failed", "Failed.", 500)]
        );
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.updated",
            null,
            "cus_1",
            "sub_1",
            DateTime.UtcNow.AddMonths(1),
            "active",
            false
        );
        var command = new HandleSubscriptionUpdatedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenDowngradeReturnsErrors_ReturnsFailure()
    {
        _downgradeResponseToReturn = new DowngradeProResponse(
            [new SharedLib.Domain.Errors.Error("Downgrade.Failed", "Failed.", 500)]
        );
        var payload = new StripeWebhookPayload(
            "evt_1",
            "customer.subscription.updated",
            null,
            "cus_1",
            "sub_1",
            null,
            "canceled",
            false
        );
        var command = new HandleSubscriptionUpdatedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}
