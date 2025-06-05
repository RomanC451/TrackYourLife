using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.VerifyEmail;

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
}
