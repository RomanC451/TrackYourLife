using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Domain.UnitTests.Features.Users.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldCreateEmail()
    {
        // Arrange
        var validEmail = "test@example.com";

        // Act
        var result = Email.Create(validEmail);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(validEmail);
    }

    [Fact]
    public void Create_WithEmptyEmail_ShouldFail()
    {
        // Arrange
        var emptyEmail = string.Empty;

        // Act
        var result = Email.Create(emptyEmail);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Email.Empty);
    }

    [Fact]
    public void Create_WithWhitespaceEmail_ShouldFail()
    {
        // Arrange
        var whitespaceEmail = "   ";

        // Act
        var result = Email.Create(whitespaceEmail);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Email.Empty);
    }

    [Fact]
    public void Create_WithTooLongEmail_ShouldFail()
    {
        // Arrange
        var tooLongEmail = new string('a', Email.MaxLength - 5) + "@test.com";

        // Act
        var result = Email.Create(tooLongEmail);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Email.TooLong);
    }

    [Fact]
    public void Create_WithInvalidFormatEmail_ShouldFail()
    {
        // Arrange
        var invalidEmail = "testexample.com";

        // Act
        var result = Email.Create(invalidEmail);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Email.InvalidFormat);
    }

    [Fact]
    public void Create_WithMultipleAtSymbols_ShouldFail()
    {
        // Arrange
        var invalidEmail = "test@example@test.com";

        // Act
        var result = Email.Create(invalidEmail);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Email.InvalidFormat);
    }

    [Fact]
    public void Create_WithMaxLengthEmail_ShouldCreateEmail()
    {
        // Arrange
        var maxLengthEmail = new string('a', Email.MaxLength - 10) + "@test.com";

        // Act
        var result = Email.Create(maxLengthEmail);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(maxLengthEmail);
    }
}
