using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.VerifyEmail;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.VerifyEmail;

public class VerifyEmailCommandValidatorTests
{
    private readonly VerifyEmailCommandValidator _validator;

    public VerifyEmailCommandValidatorTests()
    {
        _validator = new VerifyEmailCommandValidator();
    }

    [Fact]
    public void Validate_WithValidInput_ShouldBeValid()
    {
        // Arrange
        var command = new VerifyEmailCommand("12345678901234567890123456789012");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234567890123456789012345678901")] // 31 characters
    [InlineData("123456789012345678901234567890123")] // 33 characters
    [InlineData("123")] // too short
    [InlineData("123456789012345678901234567890123456789012345678901234567890123")] // too long
    public void Validate_WithInvalidTokenLength_ShouldBeInvalid(string token)
    {
        // Arrange
        var command = new VerifyEmailCommand(token);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VerificationToken);
    }
}
