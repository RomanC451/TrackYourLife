using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Domain.UnitTests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenValueIsNull()
    {
        // Arrange
        string email = null!;

        // Act
        var result = Email.Create(email);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(Error.NullValue, result.Error);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenValueIsEmpty()
    {
        // Arrange
        string email = "";

        // Act
        var result = Email.Create(email);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Email.Empty, result.Error);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenValueIsWhiteSpace()
    {
        // Arrange
        string email = " ";

        // Act
        var result = Email.Create(email);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Email.Empty, result.Error);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenValueIsTooLong()
    {
        // Arrange
        string email = new('a', Email.MaxLength + 1);

        // Act
        var result = Email.Create(email);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Email.TooLong, result.Error);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenValueHasInvalidFormat()
    {
        // Arrange
        string email = "invalid-format-email";

        // Act
        var result = Email.Create(email);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Email.InvalidFormat, result.Error);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenValueIsValid()
    {
        // Arrange
        string email = "test@example.com";

        // Act
        var result = Email.Create(email);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(email.Trim(), result.Value.Value);
    }

    [Fact]
    public void GetAtomicValues_ShouldReturnSingleValue()
    {
        // Arrange
        string email = "test@example.com";
        var emailObj = Email.Create(email).Value;

        // Act
        var atomicValues = emailObj.GetAtomicValues();

        // Assert
        Assert.Single(atomicValues);
        Assert.Equal(email.Trim(), atomicValues.First());
    }
}
