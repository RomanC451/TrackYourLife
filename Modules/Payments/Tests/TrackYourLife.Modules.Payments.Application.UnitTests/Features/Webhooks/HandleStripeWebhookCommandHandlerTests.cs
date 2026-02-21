using MediatR;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleStripeWebhook;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.Webhooks;

public sealed class HandleStripeWebhookCommandHandlerTests
{
    private readonly IStripeService _stripeService;
    private readonly ISender _sender;
    private readonly HandleStripeWebhookCommandHandler _handler;

    public HandleStripeWebhookCommandHandlerTests()
    {
        _stripeService = Substitute.For<IStripeService>();
        _sender = Substitute.For<ISender>();
        _sender
            .Send(Arg.Any<IRequest<Result>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _handler = new HandleStripeWebhookCommandHandler(_stripeService, _sender);
    }

    [Fact]
    public async Task Handle_WhenParseFails_ReturnsFailure()
    {
        _stripeService.TryParseWebhookEvent("{}", "sig").Returns((null, "Invalid signature"));

        var result = await _handler.Handle(
            new HandleStripeWebhookCommand("{}", "sig"),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeFalse();
        await _stripeService
            .DidNotReceive()
            .HasProcessedEventAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenEventAlreadyProcessed_ReturnsSuccess()
    {
        var payload = new StripeWebhookPayload(
            "evt_123",
            "checkout.session.completed",
            null,
            null,
            null,
            null,
            null
        );
        _stripeService
            .TryParseWebhookEvent(Arg.Any<string>(), Arg.Any<string>())
            .Returns((payload, null));
        _stripeService
            .HasProcessedEventAsync("evt_123", Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _handler.Handle(
            new HandleStripeWebhookCommand("{}", "sig"),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_CheckoutSessionCompleted_SendsUpgradeToProRequest()
    {
        var userId = UserId.NewId();
        var periodEnd = DateTime.UtcNow.AddMonths(1);
        var payload = new StripeWebhookPayload(
            "evt_checkout",
            "checkout.session.completed",
            userId.Value.ToString(),
            "cus_abc",
            "sub_123",
            periodEnd,
            "active"
        );
        _stripeService
            .TryParseWebhookEvent(Arg.Any<string>(), Arg.Any<string>())
            .Returns((payload, null));
        _stripeService
            .HasProcessedEventAsync("evt_checkout", Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _handler.Handle(
            new HandleStripeWebhookCommand("{}", "sig"),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        await _stripeService
            .Received(1)
            .MarkEventProcessedAsync("evt_checkout", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SubscriptionDeleted_SendsDowngradeProRequest()
    {
        var payload = new StripeWebhookPayload(
            "evt_del",
            "customer.subscription.deleted",
            null,
            "cus_xyz",
            null,
            null,
            "canceled"
        );
        _stripeService
            .TryParseWebhookEvent(Arg.Any<string>(), Arg.Any<string>())
            .Returns((payload, null));
        _stripeService
            .HasProcessedEventAsync("evt_del", Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _handler.Handle(
            new HandleStripeWebhookCommand("{}", "sig"),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        await _stripeService
            .Received(1)
            .MarkEventProcessedAsync("evt_del", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvoicePaymentFailed_ReturnsSuccess()
    {
        var payload = new StripeWebhookPayload(
            "evt_inv",
            "invoice.payment_failed",
            null,
            "cus_abc",
            null,
            null,
            null
        );
        _stripeService
            .TryParseWebhookEvent(Arg.Any<string>(), Arg.Any<string>())
            .Returns((payload, null));
        _stripeService
            .HasProcessedEventAsync("evt_inv", Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _handler.Handle(
            new HandleStripeWebhookCommand("{}", "sig"),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        await _stripeService
            .Received(1)
            .MarkEventProcessedAsync("evt_inv", Arg.Any<CancellationToken>());
    }
}
