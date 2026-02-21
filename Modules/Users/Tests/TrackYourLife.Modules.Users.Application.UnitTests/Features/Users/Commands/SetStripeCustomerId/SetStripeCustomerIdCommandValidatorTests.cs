using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.SetStripeCustomerId;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.SetStripeCustomerId;

public sealed class SetStripeCustomerIdCommandValidatorTests
{
    private readonly SetStripeCustomerIdCommandValidator _validator;

    public SetStripeCustomerIdCommandValidatorTests()
    {
        _validator = new SetStripeCustomerIdCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        var userId = UserId.NewId();
        var command = new SetStripeCustomerIdCommand(userId, "cus_ValidCustomerId");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUserIdIsDefault_ShouldHaveValidationError()
    {
        var command = new SetStripeCustomerIdCommand(default!, "cus_123");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenStripeCustomerIdIsEmpty_ShouldHaveValidationError()
    {
        var userId = UserId.NewId();
        var command = new SetStripeCustomerIdCommand(userId, "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StripeCustomerId);
    }

    [Fact]
    public void Validate_WhenStripeCustomerIdExceedsMaxLength_ShouldHaveValidationError()
    {
        var userId = UserId.NewId();
        var command = new SetStripeCustomerIdCommand(userId, new string('a', 257));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StripeCustomerId);
    }
}
