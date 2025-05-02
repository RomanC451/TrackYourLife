using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RegisterUser;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.RegisterUser;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator;

    public RegisterUserCommandValidatorTests()
    {
        _validator = new RegisterUserCommandValidator();
    }

    [Fact]
    public void Validate_WithValidInput_ShouldBeValid()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "ValidPassword123!",
            "John",
            "Doe"
        );

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
        var command = new RegisterUserCommand(email, "ValidPassword123!", "John", "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("short")]
    [InlineData("noNumbers!")]
    [InlineData("NoSpecialChars123")]
    [InlineData("12345678")]
    [InlineData("abcdefgh")]
    public void Validate_WithInvalidPasswordFormat_ShouldBeInvalid(string password)
    {
        // Arrange
        var command = new RegisterUserCommand("test@example.com", password, "John", "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithInvalidFirstNameFormat_ShouldBeInvalid(string firstName)
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "ValidPassword123!",
            firstName,
            "Doe"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithInvalidLastNameFormat_ShouldBeInvalid(string lastName)
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "ValidPassword123!",
            "John",
            lastName
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}
