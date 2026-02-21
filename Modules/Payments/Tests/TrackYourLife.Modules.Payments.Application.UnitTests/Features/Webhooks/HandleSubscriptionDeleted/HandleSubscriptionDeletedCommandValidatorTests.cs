using FluentValidation.TestHelper;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionDeleted;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.Webhooks.HandleSubscriptionDeleted;

public sealed class HandleSubscriptionDeletedCommandValidatorTests
{
    private readonly HandleSubscriptionDeletedCommandValidator _validator;

    public HandleSubscriptionDeletedCommandValidatorTests()
    {
        _validator = new HandleSubscriptionDeletedCommandValidator();
    }

    [Fact]
    public void Validate_WhenPayloadNotNull_ShouldNotHaveValidationErrors()
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

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenPayloadNull_ShouldHaveValidationError()
    {
        var command = new HandleSubscriptionDeletedCommand(default!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Payload);
    }
}
