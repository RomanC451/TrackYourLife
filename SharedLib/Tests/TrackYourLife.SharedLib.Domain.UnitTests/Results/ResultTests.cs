using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Domain.UnitTests.Results;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Success_WithValue_ShouldCreateSuccessfulResultWithValue()
    {
        // Arrange
        var value = "test";

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
        result.Value.Should().Be(value);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Failure_WithValue_ShouldCreateFailedResultWithDefaultValue()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;

        // Act
        var result = Result.Failure<string>(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnFailure()
    {
        // Act
        var result = Result.Create<string>(null);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }

    [Fact]
    public void Create_WithNullValueAndCustomError_ShouldReturnFailureWithCustomError()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;

        // Act
        var result = Result.Create<string>(null, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void FirstFailureOrSuccess_WithAllSuccesses_ShouldReturnSuccess()
    {
        // Arrange
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Success();

        // Act
        var result = Result.FirstFailureOrSuccess(result1, result2, result3);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void FirstFailureOrSuccess_WithFirstFailure_ShouldReturnFirstFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var result1 = Result.Failure(error);
        var result2 = Result.Success();
        var result3 = Result.Success();

        // Act
        var result = Result.FirstFailureOrSuccess(result1, result2, result3);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void FirstFailureOrSuccess_WithLastFailure_ShouldReturnLastFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Failure(error);

        // Act
        var result = Result.FirstFailureOrSuccess(result1, result2, result3);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Value_OnFailedResult_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var result = Result.Failure<string>(DomainErrors.General.UnProcessableRequest);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => result.Value);
        exception.Message.Should().Be("The value of a failure result can not be accessed.");
    }

    [Fact]
    public void ImplicitOperator_WithNonNullValue_ShouldCreateSuccessResult()
    {
        // Arrange
        string value = "test";

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
        result.Value.Should().Be(value);
    }

    [Fact]
    public void ImplicitOperator_WithNullValue_ShouldCreateFailureResult()
    {
        // Arrange
        string? value = null;

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }

    [Fact]
    public void Create_WithNonNullValue_ShouldPreserveValue()
    {
        // Arrange
        var complexValue = new { Name = "Test", Value = 42 };

        // Act
        var result = Result.Create(complexValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(complexValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("test")]
    public void Create_WithNonNullString_ShouldCreateSuccessResult(string value)
    {
        // Act
        var result = Result.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }
}
