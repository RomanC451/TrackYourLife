using TrackYourLife.Domain.Shared;
using Xunit;

namespace TrackYourLife.Common.Domain.UnitTests.Shared;

public class ErrorTests
{
    [Fact]
    public void Error_DefaultConstructor_ShouldCreateEmptyObject()
    {
        var error = new Error("", "");
        Assert.Equal("", error.Code);
        Assert.Equal("", error.Message);
    }

    [Fact]
    public void Error_ImplicitConversionToString_ShouldReturnCode()
    {
        var error = new Error("test", "Test error");
        string code = error;
        Assert.Equal("test", code);
    }

    [Fact]
    public void Error_Equals_ShouldReturnTrueForEqualErrors()
    {
        var error1 = new Error("test", "Test error");
        var error2 = new Error("test", "Test error");
        Assert.True(error1.Equals(error2));
    }

    [Fact]
    public void Error_Equals_ShouldReturnFalseForDifferentErrors()
    {
        var error1 = new Error("test1", "Test error 1");
        var error2 = new Error("test2", "Test error 2");
        Assert.False(error1.Equals(error2));
    }

    [Fact]
    public void Error_Equals_ShouldReturnFalseForNullInput()
    {
        var error = new Error("test", "Test error");
        Assert.False(error.Equals(null));
    }

    [Fact]
    public void Error_GetHashCode_ShouldReturnSameHashCodeForEqualErrors()
    {
        var error1 = new Error("test", "Test error");
        var error2 = new Error("test", "Test error");
        Assert.Equal(error1.GetHashCode(), error2.GetHashCode());
    }

    [Fact]
    public void Error_GetHashCode_ShouldReturnDifferentHashCodeForDifferentErrors()
    {
        var error1 = new Error("test1", "Test error 1");
        var error2 = new Error("test2", "Test error 2");
        Assert.NotEqual(error1.GetHashCode(), error2.GetHashCode());
    }

    [Fact]
    public void Error_Operators_ShouldReturnTrueForEqualErrors()
    {
        var error1 = new Error("test", "Test error");
        var error2 = new Error("test", "Test error");
        Assert.True(error1 == error2);
        Assert.False(error1 != error2);
    }

    [Fact]
    public void Error_Operators_ShouldReturnFalseForDifferentErrors()
    {
        var error1 = new Error("test1", "Test error 1");
        var error2 = new Error("test2", "Test error 2");
        Assert.False(error1 == error2);
        Assert.True(error1 != error2);
    }

    [Fact]
    public void Error_Operators_ShouldReturnFalseForNullInput()
    {
        var error = new Error("test", "Test error");
        Assert.False(error == null);
        Assert.True(error != null);
    }

    [Fact]
    public void Error_Should_Be_Equal_To_Self()
    {
        // Arrange
        var error = new Error("Test.Code", "Test Message");

        // Act
        var result = error.Equals(error);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Error_Should_Not_Be_Equal_To_Null()
    {
        // Arrange
        var error = new Error("Test.Code", "Test Message");

        // Act
        var result = error.Equals(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Error_Should_Not_Be_Equal_To_Other_Type()
    {
        // Arrange
        var error = new Error("Test.Code", "Test Message");

        // Act
        var result = error.Equals("Not an Error object");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Errors_With_Same_Code_And_Message_Should_Be_Equal()
    {
        // Arrange
        var error1 = new Error("Test.Code", "Test Message");
        var error2 = new Error("Test.Code", "Test Message");

        // Act
        var result = error1.Equals(error2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Errors_With_Different_Code_Should_Not_Be_Equal()
    {
        // Arrange
        var error1 = new Error("Test.Code1", "Test Message");
        var error2 = new Error("Test.Code2", "Test Message");

        // Act
        var result = error1.Equals(error2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Errors_With_Different_Message_Should_Not_Be_Equal()
    {
        // Arrange
        var error1 = new Error("Test.Code", "Test Message 1");
        var error2 = new Error("Test.Code", "Test Message 2");

        // Act
        var result = error1.Equals(error2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Error_Should_Be_Equal_To_Same_Code()
    {
        // Arrange
        var error = new Error("Test.Code", "Test Message");

        // Act
        var result = error == "Test.Code";

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Error_Should_Not_Be_Equal_To_Different_Code()
    {
        // Arrange
        var error = new Error("Test.Code", "Test Message");

        // Act
        var result = error == "Other.Code";

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Error_Should_Have_Same_Hash_Code_As_Same_Code_And_Message()
    {
        // Arrange
        var error1 = new Error("Test.Code", "Test Message");
        var error2 = new Error("Test.Code", "Test Message");

        // Act
        var result = error1.GetHashCode() == error2.GetHashCode();

        // Assert
        Assert.True(result);
    }
}
