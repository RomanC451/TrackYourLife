using FluentValidation.TestHelper;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionUpdated;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.Webhooks.HandleSubscriptionUpdated;

public sealed class HandleSubscriptionUpdatedCommandValidatorTests
{
    private readonly HandleSubscriptionUpdatedCommandValidator _validator;

    public HandleSubscriptionUpdatedCommandValidatorTests()
    {
        _validator = new HandleSubscriptionUpdatedCommandValidator();
    }

    [Fact]
    public void Validate_WhenPayloadNotNull_ShouldNotHaveValidationErrors()
    {
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

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenPayloadNull_ShouldHaveValidationError()
    {
        var command = new HandleSubscriptionUpdatedCommand(default!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Payload);
    }
}
