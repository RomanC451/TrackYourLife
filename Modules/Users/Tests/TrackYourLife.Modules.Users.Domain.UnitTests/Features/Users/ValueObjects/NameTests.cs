using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Domain.UnitTests.Features.Users.ValueObjects;

public class NameTests
{
    [Fact]
    public void Create_WithValidName_ShouldCreateName()
    {
        // Arrange
        var validName = "John";

        // Act
        var result = Name.Create(validName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(validName);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldFail()
    {
        // Arrange
        var emptyName = string.Empty;

        // Act
        var result = Name.Create(emptyName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Name.Empty);
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldFail()
    {
        // Arrange
        var whitespaceName = "   ";

        // Act
        var result = Name.Create(whitespaceName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Name.Empty);
    }

    [Fact]
    public void Create_WithTooLongName_ShouldFail()
    {
        // Arrange
        var tooLongName = new string('a', Name.MaxLength + 1);

        // Act
        var result = Name.Create(tooLongName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Name.TooLong);
    }

    [Fact]
    public void Create_WithMaxLengthName_ShouldCreateName()
    {
        // Arrange
        var maxLengthName = new string('a', Name.MaxLength);

        // Act
        var result = Name.Create(maxLengthName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(maxLengthName);
    }
}
