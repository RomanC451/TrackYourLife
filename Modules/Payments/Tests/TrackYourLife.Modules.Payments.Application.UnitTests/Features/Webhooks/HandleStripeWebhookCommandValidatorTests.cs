using FluentValidation.TestHelper;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleStripeWebhook;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.Webhooks;

public sealed class HandleStripeWebhookCommandValidatorTests
{
    private readonly HandleStripeWebhookCommandValidator _validator;

    public HandleStripeWebhookCommandValidatorTests()
    {
        _validator = new HandleStripeWebhookCommandValidator();
    }

    [Fact]
    public void Validate_WhenJsonPayloadAndSignatureProvided_ShouldNotHaveValidationErrors()
    {
        var command = new HandleStripeWebhookCommand("{\"type\":\"event\"}", "t=123,v1=abc");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenJsonPayloadEmpty_ShouldHaveValidationError()
    {
        var command = new HandleStripeWebhookCommand("", "t=123,v1=abc");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.JsonPayload);
    }

    [Fact]
    public void Validate_WhenSignatureHeaderEmpty_ShouldHaveValidationError()
    {
        var command = new HandleStripeWebhookCommand("{\"type\":\"event\"}", "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SignatureHeader);
    }
}
