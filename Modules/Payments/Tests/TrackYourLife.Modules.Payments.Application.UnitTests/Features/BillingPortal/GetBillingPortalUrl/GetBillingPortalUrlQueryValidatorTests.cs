using FluentValidation.TestHelper;
using TrackYourLife.Modules.Payments.Application.Features.BillingPortal.GetBillingPortalUrl;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Features.BillingPortal.GetBillingPortalUrl;

public sealed class GetBillingPortalUrlQueryValidatorTests
{
    private readonly GetBillingPortalUrlQueryValidator _validator;

    public GetBillingPortalUrlQueryValidatorTests()
    {
        _validator = new GetBillingPortalUrlQueryValidator();
    }

    [Fact]
    public void Validate_WhenReturnUrlIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetBillingPortalUrlQuery("https://app.example.com/billing");

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenReturnUrlIsEmpty_ShouldHaveValidationError()
    {
        var query = new GetBillingPortalUrlQuery("");

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.ReturnUrl);
    }

    [Fact]
    public void Validate_WhenReturnUrlExceedsMaximumLength_ShouldHaveValidationError()
    {
        var query = new GetBillingPortalUrlQuery(new string('a', 2049));

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.ReturnUrl);
    }

    [Fact]
    public void Validate_WhenReturnUrlIsExactlyMaxLength_ShouldNotHaveValidationError()
    {
        var query = new GetBillingPortalUrlQuery(new string('a', 2048));

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveValidationErrorFor(x => x.ReturnUrl);
    }
}
