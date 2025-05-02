using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.ResendVerificationEmail;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.ResendVerificationEmail;

public class ResendEmailVerificationCommandValidatorTests
{
    private readonly ResendEmailVerificationCommandValidator _validator;

    public ResendEmailVerificationCommandValidatorTests()
    {
        _validator = new ResendEmailVerificationCommandValidator();
    }

    [Fact]
    public void Validate_WithValidInput_ShouldBeValid()
    {
        // Arrange
        var command = new ResendEmailVerificationCommand("test@example.com");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    public void Validate_WithInvalidEmailFormat_ShouldBeInvalid(string email)
    {
        // Arrange
        var command = new ResendEmailVerificationCommand(email);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
