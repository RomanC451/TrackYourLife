using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Users.ValueObjects;
using Xunit;

namespace TrackYourLife.Common.Domain.UnitTests.ValueObjects;

public class FirstNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WhenFirstNameIsNullOrWhitespace_ReturnsFailure(string firstName)
    {
        // Act
        var result = Name.Create(firstName);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Name.Empty, result.Error);
    }

    [Fact]
    public void Create_WhenFirstNameLengthIsGreaterThanMaxLength_ReturnsFailure()
    {
        // Arrange
        var firstName = new string('a', Name.MaxLength + 1);

        // Act
        var result = Name.Create(firstName);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Name.TooLong, result.Error);
    }

    [Fact]
    public void Create_WhenFirstNameIsValid_ReturnsSuccess()
    {
        // Arrange
        var firstName = "John";

        // Act
        var result = Name.Create(firstName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(firstName, result.Value.Value);
    }
}
