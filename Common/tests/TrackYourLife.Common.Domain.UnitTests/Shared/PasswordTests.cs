using Xunit;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Users.ValueObjects;
using TrackYourLife.Common.Domain.Errors;

namespace TrackYourLife.Common.Domain.UnitTests.Shared;

public class PasswordTests
{
    private const string ValidPassword = "MyValidPassword123!";

    [Fact]
    public void Create_ValidPassword_ReturnsSuccessResult()
    {
        // Arrange
        var password = ValidPassword;

        // Act
        var result = Password.Create(password);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_NullPassword_ReturnsFailureResultWithEmptyError()
    {
        // Act
        var result = Password.Create(null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(Error.NullValue, result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_EmptyPassword_ReturnsFailureResultWithEmptyError(string password)
    {
        // Act
        var result = Password.Create(password);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(DomainErrors.Password.Empty, result.Error);
    }

    [Theory]
    [InlineData("abc", "Password.TooShort")]
    [InlineData("MYINVALIDPASSWORD123!", "Password.LowerCase")]
    [InlineData("myinvalidpassword123!", "Password.UpperCase")]
    [InlineData("MyInvalidPassword", "Password.Number")]
    [InlineData("MyInvalidPassword123", "Password.Symbol")]
    public void Create_InvalidPassword_ReturnsFailureResultWithCorespondingCode(
        string password,
        string error
    )
    {
        // Act
        var result = Password.Create(password);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Error.Code);
    }

    [Fact]
    public void GetAtomicValues_ReturnsSingleValue()
    {
        // Arrange
        var passwordObject = Password.Create(ValidPassword).Value;

        // Act
        var atomicValues = passwordObject.GetAtomicValues();

        // Assert
        Assert.Single(atomicValues);
        Assert.Equal(ValidPassword, atomicValues.First());
    }
}
