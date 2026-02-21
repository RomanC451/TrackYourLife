using FluentValidation.TestHelper;
using TrackYourLife.Modules.Payments.Application.Features.Checkout.CreateCheckoutSession;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.Checkout.CreateCheckoutSession;

public sealed class CreateCheckoutSessionCommandValidatorTests
{
    private readonly CreateCheckoutSessionCommandValidator _validator;

    public CreateCheckoutSessionCommandValidatorTests()
    {
        _validator = new CreateCheckoutSessionCommandValidator();
    }

    [Fact]
    public void Validate_WhenAllFieldsValid_ShouldNotHaveValidationErrors()
    {
        var command = new CreateCheckoutSessionCommand(
            "https://app.example.com/success",
            "https://app.example.com/cancel",
            "price_123"
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenSuccessUrlEmpty_ShouldHaveValidationError()
    {
        var command = new CreateCheckoutSessionCommand("", "https://app/cancel", "price_123");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SuccessUrl);
    }

    [Fact]
    public void Validate_WhenCancelUrlEmpty_ShouldHaveValidationError()
    {
        var command = new CreateCheckoutSessionCommand("https://app/success", "", "price_123");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CancelUrl);
    }

    [Fact]
    public void Validate_WhenPriceIdEmpty_ShouldHaveValidationError()
    {
        var command = new CreateCheckoutSessionCommand(
            "https://app/success",
            "https://app/cancel",
            ""
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PriceId);
    }

    [Fact]
    public void Validate_WhenSuccessUrlExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new CreateCheckoutSessionCommand(
            new string('a', 2049),
            "https://app/cancel",
            "price_123"
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SuccessUrl);
    }

    [Fact]
    public void Validate_WhenCancelUrlExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new CreateCheckoutSessionCommand(
            "https://app/success",
            new string('a', 2049),
            "price_123"
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CancelUrl);
    }

    [Fact]
    public void Validate_WhenPriceIdExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new CreateCheckoutSessionCommand(
            "https://app/success",
            "https://app/cancel",
            new string('a', 257)
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PriceId);
    }

    [Fact]
    public void Validate_WhenUrlsAndPriceIdAtMaxLength_ShouldNotHaveValidationErrors()
    {
        var command = new CreateCheckoutSessionCommand(
            new string('a', 2048),
            new string('a', 2048),
            new string('a', 256)
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
