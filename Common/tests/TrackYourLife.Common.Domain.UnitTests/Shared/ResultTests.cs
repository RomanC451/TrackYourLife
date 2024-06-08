using TrackYourLife.Common.Domain.Shared;
using Xunit;

namespace TrackYourLife.Common.Domain.UnitTests.Shared;

public class ResultTests
{
    [Fact]
    public void Success_Should_Create_Successful_Result()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("", result.Error.Code);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Success_With_Value_Should_Create_Successful_Result_With_Value()
    {
        // Arrange
        var value = 42;

        // Act
        var result = Result.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("", result.Error.Code);
        Assert.Equal(value, result.Value);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_Should_Create_Failure_Result_With_Error()
    {
        // Arrange
        var error = new Error("code", "message");

        // Act
        var result = Result.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Failure_With_Value_Should_Create_Failure_Result_With_Error()
    {
        // Arrange
        var error = new Error("code", "message");

        // Act
        var result = Result.Failure<int>(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Create_Should_Return_Success_When_Value_Is_Not_Null()
    {
        // Arrange
        var value = 42;

        // Act
        var result = Result.Create(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("", result.Error.Code);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Create_Should_Return_Failure_With_NullValue_Error_When_Value_Is_Null()
    {
        // Arrange
        int? value = null;

        // Act
        var result = Result.Create(value);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.NullValue, result.Error);
    }
}
