using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.DowngradePro;
using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.DowngradePro;

public sealed class DowngradeProCommandValidatorTests
{
    private readonly DowngradeProCommandValidator _validator;

    public DowngradeProCommandValidatorTests()
    {
        _validator = new DowngradeProCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        var userId = UserId.NewId();
        var command = new DowngradeProCommand(userId, SubscriptionStatus.Canceled);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUserIdIsDefault_ShouldHaveValidationError()
    {
        var command = new DowngradeProCommand(default!, SubscriptionStatus.Canceled);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenSubscriptionStatusIsOutOfRange_ShouldHaveValidationError()
    {
        var userId = UserId.NewId();
        var command = new DowngradeProCommand(userId, (SubscriptionStatus)999);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SubscriptionStatus);
    }
}
