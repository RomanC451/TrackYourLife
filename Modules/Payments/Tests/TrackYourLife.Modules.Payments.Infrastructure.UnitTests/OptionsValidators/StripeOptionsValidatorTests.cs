using FluentValidation.TestHelper;
using TrackYourLife.Modules.Payments.Infrastructure.Options;
using TrackYourLife.Modules.Payments.Infrastructure.OptionsValidators;

namespace TrackYourLife.Modules.Payments.Infrastructure.UnitTests.OptionsValidators;

public sealed class StripeOptionsValidatorTests
{
    private readonly StripeOptionsValidator _validator;

    public StripeOptionsValidatorTests()
    {
        _validator = new StripeOptionsValidator();
    }

    [Fact]
    public void Validate_WhenAllRequiredFieldsAreSet_ShouldNotHaveValidationErrors()
    {
        var options = new StripeOptions
        {
            SecretKey = "sk_test_abc",
            WebhookSecret = "whsec_xyz",
            ProPriceIdMonthly = "price_monthly_123",
            ProPriceIdYearly = "price_yearly_456",
        };

        var result = _validator.TestValidate(options);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenWebhookSecretIsEmpty_ShouldHaveValidationError()
    {
        var options = new StripeOptions
        {
            SecretKey = "sk_test_abc",
            WebhookSecret = "",
            ProPriceIdMonthly = "price_monthly_123",
            ProPriceIdYearly = "price_yearly_456",
        };

        var result = _validator.TestValidate(options);

        result.ShouldHaveValidationErrorFor(x => x.WebhookSecret);
    }

    [Fact]
    public void Validate_WhenProPriceIdMonthlyIsEmpty_ShouldHaveValidationError()
    {
        var options = new StripeOptions
        {
            SecretKey = "sk_test_abc",
            WebhookSecret = "whsec_xyz",
            ProPriceIdMonthly = "",
            ProPriceIdYearly = "price_yearly_456",
        };

        var result = _validator.TestValidate(options);

        result.ShouldHaveValidationErrorFor(x => x.ProPriceIdMonthly);
    }

    [Fact]
    public void Validate_WhenProPriceIdYearlyIsEmpty_ShouldHaveValidationError()
    {
        var options = new StripeOptions
        {
            SecretKey = "sk_test_abc",
            WebhookSecret = "whsec_xyz",
            ProPriceIdMonthly = "price_monthly_123",
            ProPriceIdYearly = "",
        };

        var result = _validator.TestValidate(options);

        result.ShouldHaveValidationErrorFor(x => x.ProPriceIdYearly);
    }

    [Fact]
    public void Validate_WhenMultipleFieldsAreEmpty_ShouldHaveValidationErrorsForEach()
    {
        var options = new StripeOptions
        {
            SecretKey = "sk_test_abc",
            WebhookSecret = "",
            ProPriceIdMonthly = "",
            ProPriceIdYearly = "",
        };

        var result = _validator.TestValidate(options);

        result.ShouldHaveValidationErrorFor(x => x.WebhookSecret);
        result.ShouldHaveValidationErrorFor(x => x.ProPriceIdMonthly);
        result.ShouldHaveValidationErrorFor(x => x.ProPriceIdYearly);
    }
}
