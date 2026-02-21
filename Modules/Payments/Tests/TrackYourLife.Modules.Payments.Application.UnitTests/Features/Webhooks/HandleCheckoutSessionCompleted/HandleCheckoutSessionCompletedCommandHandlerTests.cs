using MassTransit;
using MassTransit.Testing;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleCheckoutSessionCompleted;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.Webhooks.HandleCheckoutSessionCompleted;

public sealed class HandleCheckoutSessionCompletedCommandHandlerTests : IAsyncLifetime
{
    private InMemoryTestHarness _harness = null!;
    private IBus _bus = null!;
    private HandleCheckoutSessionCompletedCommandHandler _handler = null!;
    private UpgradeToProResponse? _upgradeResponseToReturn;

    public async Task InitializeAsync()
    {
        _harness = new InMemoryTestHarness();
        _upgradeResponseToReturn = new UpgradeToProResponse([]);
        _harness.OnConfigureBus += configurator =>
        {
            configurator.ReceiveEndpoint(
                "upgrade-to-pro",
                e =>
                {
                    e.Handler<UpgradeToProRequest>(async context =>
                    {
                        var response = _upgradeResponseToReturn ?? new UpgradeToProResponse([]);
                        await context.RespondAsync(response);
                    });
                }
            );
        };
        await _harness.Start();
        _bus = _harness.Bus;
        _handler = new HandleCheckoutSessionCompletedCommandHandler(_bus);
    }

    public async Task DisposeAsync() => await _harness.Stop();

    [Fact]
    public async Task Handle_WhenPayloadNull_ReturnsFailure()
    {
        var command = new HandleCheckoutSessionCompletedCommand(default!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("MissingPayload");
    }

    [Fact]
    public async Task Handle_WhenClientReferenceIdInvalid_ReturnsFailure()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "checkout.session.completed",
            "not-a-guid",
            "cus_1",
            "sub_1",
            DateTime.UtcNow.AddMonths(1),
            "active",
            false
        );
        var command = new HandleCheckoutSessionCompletedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("InvalidClientReferenceId");
    }

    [Fact]
    public async Task Handle_WhenCustomerIdEmpty_ReturnsFailure()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "checkout.session.completed",
            Guid.NewGuid().ToString(),
            "",
            "sub_1",
            DateTime.UtcNow.AddMonths(1),
            "active",
            false
        );
        var command = new HandleCheckoutSessionCompletedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("MissingCustomerId");
    }

    [Fact]
    public async Task Handle_WhenCurrentPeriodEndUtcNull_ReturnsFailure()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "checkout.session.completed",
            Guid.NewGuid().ToString(),
            "cus_1",
            "sub_1",
            null,
            "active",
            false
        );
        var command = new HandleCheckoutSessionCompletedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("MissingCurrentPeriodEnd");
    }

    [Fact]
    public async Task Handle_WhenValidPayload_ReturnsSuccess()
    {
        var userId = UserId.NewId();
        var periodEnd = DateTime.UtcNow.AddMonths(1);
        var payload = new StripeWebhookPayload(
            "evt_1",
            "checkout.session.completed",
            userId.Value.ToString(),
            "cus_abc",
            "sub_123",
            periodEnd,
            "active",
            false
        );
        var command = new HandleCheckoutSessionCompletedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenUpgradeReturnsErrors_ReturnsFailure()
    {
        _upgradeResponseToReturn = new UpgradeToProResponse(
            [new SharedLib.Domain.Errors.Error("User.NotFound", "User not found.", 404)]
        );
        var userId = UserId.NewId();
        var payload = new StripeWebhookPayload(
            "evt_1",
            "checkout.session.completed",
            userId.Value.ToString(),
            "cus_abc",
            "sub_123",
            DateTime.UtcNow.AddMonths(1),
            "active",
            false
        );
        var command = new HandleCheckoutSessionCompletedCommand(payload);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("User.NotFound");
    }
}
