using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogInUser;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.LogInUser;

public class LogInUserCommandValidatorTests
{
    private readonly LoginUserCommandValidator _validator;

    public LogInUserCommandValidatorTests()
    {
        _validator = new LoginUserCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new LogInUserCommand("test@example.com", "ValidPass123!", DeviceId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [InlineData("test@example@test.com")]
    public void Validate_WithInvalidEmail_ShouldHaveValidationError(string email)
    {
        // Arrange
        var command = new LogInUserCommand(email, "ValidPass123!", DeviceId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("no-uppercase1!")]
    [InlineData("NO-LOWERCASE1!")]
    [InlineData("NoNumbers!")]
    [InlineData("NoSpecialChar123")]
    public void Validate_WithInvalidPassword_ShouldHaveValidationError(string password)
    {
        // Arrange
        var command = new LogInUserCommand("test@example.com", password, DeviceId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_WithEmptyDeviceId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new LogInUserCommand("test@example.com", "ValidPass123!", DeviceId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeviceId);
    }
}
