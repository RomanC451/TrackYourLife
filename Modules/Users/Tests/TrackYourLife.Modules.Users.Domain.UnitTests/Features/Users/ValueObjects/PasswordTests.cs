using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Domain.UnitTests.Features.Users.ValueObjects;

public class PasswordTests
{
    [Fact]
    public void Create_WithValidPassword_ShouldCreatePassword()
    {
        // Arrange
        var validPassword = "ValidPass123!";

        // Act
        var result = Password.Create(validPassword);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(validPassword);
    }

    [Fact]
    public void Create_WithEmptyPassword_ShouldFail()
    {
        // Arrange
        var emptyPassword = string.Empty;

        // Act
        var result = Password.Create(emptyPassword);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Password.Empty);
    }

    [Fact]
    public void Create_WithWhitespacePassword_ShouldFail()
    {
        // Arrange
        var whitespacePassword = "   ";

        // Act
        var result = Password.Create(whitespacePassword);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Password.Empty);
    }

    [Fact]
    public void Create_WithTooShortPassword_ShouldFail()
    {
        // Arrange
        var tooShortPassword = "Short1!";

        // Act
        var result = Password.Create(tooShortPassword);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Password.TooShort);
    }

    [Fact]
    public void Create_WithNoLowerCasePassword_ShouldFail()
    {
        // Arrange
        var noLowerCasePassword = "PASSWORD123!";

        // Act
        var result = Password.Create(noLowerCasePassword);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Password.LowerCase);
    }

    [Fact]
    public void Create_WithNoUpperCasePassword_ShouldFail()
    {
        // Arrange
        var noUpperCasePassword = "password123!";

        // Act
        var result = Password.Create(noUpperCasePassword);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Password.UpperCase);
    }

    [Fact]
    public void Create_WithNoNumberPassword_ShouldFail()
    {
        // Arrange
        var noNumberPassword = "Password!@#$%";

        // Act
        var result = Password.Create(noNumberPassword);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Password.Number);
    }

    [Fact]
    public void Create_WithNoSpecialCharacterPassword_ShouldFail()
    {
        // Arrange
        var noSpecialCharacterPassword = "Password123";

        // Act
        var result = Password.Create(noSpecialCharacterPassword);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Password.SpecialCharacter);
    }

    [Fact]
    public void Create_WithMinLengthPassword_ShouldCreatePassword()
    {
        // Arrange
        var minLengthPassword = "Pass123!@#";

        // Act
        var result = Password.Create(minLengthPassword);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(minLengthPassword);
    }
}
