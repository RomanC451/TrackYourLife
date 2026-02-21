using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpgradeToPro;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.UpgradeToPro;

public sealed class UpgradeToProCommandValidatorTests
{
    private readonly UpgradeToProCommandValidator _validator;

    public UpgradeToProCommandValidatorTests()
    {
        _validator = new UpgradeToProCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        var userId = UserId.NewId();
        var command = new UpgradeToProCommand(
            userId,
            "cus_ValidCustomerId",
            DateTime.UtcNow.AddMonths(1)
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStripeCustomerIdIsEmpty_ShouldHaveValidationError()
    {
        var userId = UserId.NewId();
        var command = new UpgradeToProCommand(userId, "", DateTime.UtcNow.AddMonths(1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StripeCustomerId);
    }

    [Fact]
    public void Validate_WhenPeriodEndUtcIsDefault_ShouldHaveValidationError()
    {
        var userId = UserId.NewId();
        var command = new UpgradeToProCommand(userId, "cus_abc", default);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PeriodEndUtc);
    }
}
