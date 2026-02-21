using FluentValidation.TestHelper;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleCheckoutSessionCompleted;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.Webhooks.HandleCheckoutSessionCompleted;

public sealed class HandleCheckoutSessionCompletedCommandValidatorTests
{
    private readonly HandleCheckoutSessionCompletedCommandValidator _validator;

    public HandleCheckoutSessionCompletedCommandValidatorTests()
    {
        _validator = new HandleCheckoutSessionCompletedCommandValidator();
    }

    [Fact]
    public void Validate_WhenPayloadValid_ShouldNotHaveValidationErrors()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "checkout.session.completed",
            Guid.NewGuid().ToString(),
            "cus_1",
            "sub_1",
            DateTime.UtcNow.AddMonths(1),
            "active",
            false
        );
        var command = new HandleCheckoutSessionCompletedCommand(payload);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenPayloadNull_ShouldHaveValidationError()
    {
        var command = new HandleCheckoutSessionCompletedCommand(default!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Payload);
    }

    [Fact]
    public void Validate_WhenClientReferenceIdEmpty_ShouldHaveValidationError()
    {
        var payload = new StripeWebhookPayload(
            "evt_1",
            "checkout.session.completed",
            "",
            "cus_1",
            "sub_1",
            DateTime.UtcNow.AddMonths(1),
            "active",
            false
        );
        var command = new HandleCheckoutSessionCompletedCommand(payload);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Payload!.ClientReferenceId);
    }

    [Fact]
    public void Validate_WhenClientReferenceIdNotValidGuid_ShouldHaveValidationError()
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

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Payload!.ClientReferenceId);
    }

    [Fact]
    public void Validate_WhenCustomerIdEmpty_ShouldHaveValidationError()
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

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Payload!.CustomerId);
    }

    [Fact]
    public void Validate_WhenCurrentPeriodEndUtcNull_ShouldHaveValidationError()
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

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Payload!.CurrentPeriodEndUtc);
    }
}
