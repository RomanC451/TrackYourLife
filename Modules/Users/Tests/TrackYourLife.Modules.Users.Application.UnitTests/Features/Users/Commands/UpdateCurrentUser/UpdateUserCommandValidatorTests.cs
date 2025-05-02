using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateCurrentUser;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.UpdateCurrentUser;

public sealed class UpdateUserCommandValidatorTests
{
    private readonly UpdateUserCommandValidator _validator;

    public UpdateUserCommandValidatorTests()
    {
        _validator = new UpdateUserCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdateUserCommand("John", "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("", "Doe")]
    [InlineData(null, "Doe")]
    [InlineData(" ", "Doe")]
    public void Validate_WhenFirstNameIsInvalid_ShouldHaveValidationError(
        string? firstName,
        string lastName
    )
    {
        // Arrange
        var command = new UpdateUserCommand(firstName!, lastName);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("John", "")]
    [InlineData("John", null)]
    [InlineData("John", " ")]
    public void Validate_WhenLastNameIsInvalid_ShouldHaveValidationError(
        string firstName,
        string? lastName
    )
    {
        // Arrange
        var command = new UpdateUserCommand(firstName, lastName!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}
