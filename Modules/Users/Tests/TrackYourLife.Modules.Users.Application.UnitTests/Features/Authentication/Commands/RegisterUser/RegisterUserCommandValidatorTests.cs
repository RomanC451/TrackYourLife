using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RegisterUser;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.RegisterUser;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator;
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandValidatorTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _userRepository
            .IsEmailUniqueAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _validator = new RegisterUserCommandValidator(_userRepository);
    }

    [Fact]
    public async Task Validate_WithValidInput_ShouldBeValid()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "ValidPassword123!",
            "John",
            "Doe"
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    public async Task Validate_WithInvalidEmailFormat_ShouldBeInvalid(string email)
    {
        // Arrange
        var command = new RegisterUserCommand(email, "ValidPassword123!", "John", "Doe");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Validate_WithDuplicateEmail_ShouldBeInvalid()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "ValidPassword123!",
            "John",
            "Doe"
        );

        _userRepository
            .IsEmailUniqueAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage(UserErrors.Email.AlreadyUsed.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("short")]
    [InlineData("noNumbers!")]
    [InlineData("NoSpecialChars123")]
    [InlineData("12345678")]
    [InlineData("abcdefgh")]
    public async Task Validate_WithInvalidPasswordFormat_ShouldBeInvalid(string password)
    {
        // Arrange
        var command = new RegisterUserCommand("test@example.com", password, "John", "Doe");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_WithInvalidFirstNameFormat_ShouldBeInvalid(string firstName)
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "ValidPassword123!",
            firstName,
            "Doe"
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_WithInvalidLastNameFormat_ShouldBeInvalid(string lastName)
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "ValidPassword123!",
            "John",
            lastName
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}
