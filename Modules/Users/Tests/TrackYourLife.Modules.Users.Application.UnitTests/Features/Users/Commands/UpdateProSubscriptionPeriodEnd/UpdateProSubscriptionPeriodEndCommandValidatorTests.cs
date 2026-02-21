using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateProSubscriptionPeriodEnd;
using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.UpdateProSubscriptionPeriodEnd;

public sealed class UpdateProSubscriptionPeriodEndCommandValidatorTests
{
    private readonly UpdateProSubscriptionPeriodEndCommandValidator _validator;

    public UpdateProSubscriptionPeriodEndCommandValidatorTests()
    {
        _validator = new UpdateProSubscriptionPeriodEndCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        var userId = UserId.NewId();
        var command = new UpdateProSubscriptionPeriodEndCommand(
            userId,
            DateTime.UtcNow.AddMonths(1),
            SubscriptionStatus.Active,
            false
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUserIdIsDefault_ShouldHaveValidationError()
    {
        var command = new UpdateProSubscriptionPeriodEndCommand(
            default!,
            DateTime.UtcNow.AddMonths(1),
            SubscriptionStatus.Active,
            false
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenPeriodEndUtcIsDefault_ShouldHaveValidationError()
    {
        var userId = UserId.NewId();
        var command = new UpdateProSubscriptionPeriodEndCommand(
            userId,
            default,
            SubscriptionStatus.Active,
            false
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PeriodEndUtc);
    }

    [Fact]
    public void Validate_WhenSubscriptionStatusIsOutOfRange_ShouldHaveValidationError()
    {
        var userId = UserId.NewId();
        var command = new UpdateProSubscriptionPeriodEndCommand(
            userId,
            DateTime.UtcNow.AddMonths(1),
            (SubscriptionStatus)999,
            false
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SubscriptionStatus);
    }
}
