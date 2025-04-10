using FluentAssertions;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Domain.UnitTests.Results;

public class ResultExtensionsTests
{
    [Fact]
    public void Ensure_WithSuccessAndTruePredicate_ShouldReturnSuccess()
    {
        // Arrange
        var result = Result.Success(42);
        var error = DomainErrors.General.UnProcessableRequest;

        // Act
        var ensuredResult = result.Ensure(x => x > 0, error);

        // Assert
        ensuredResult.IsSuccess.Should().BeTrue();
        ensuredResult.IsFailure.Should().BeFalse();
        ensuredResult.Value.Should().Be(42);
    }

    [Fact]
    public void Ensure_WithSuccessAndFalsePredicate_ShouldReturnFailure()
    {
        // Arrange
        var result = Result.Success(42);
        var error = DomainErrors.General.UnProcessableRequest;

        // Act
        var ensuredResult = result.Ensure(x => x < 0, error);

        // Assert
        ensuredResult.IsSuccess.Should().BeFalse();
        ensuredResult.IsFailure.Should().BeTrue();
        ensuredResult.Error.Should().Be(error);
    }

    [Fact]
    public void Ensure_WithFailure_ShouldReturnOriginalFailure()
    {
        // Arrange
        var originalError = DomainErrors.General.UnProcessableRequest;
        var result = Result.Failure<int>(originalError);
        var newError = DomainErrors.General.ServerError;

        // Act
        var ensuredResult = result.Ensure(x => x > 0, newError);

        // Assert
        ensuredResult.IsSuccess.Should().BeFalse();
        ensuredResult.IsFailure.Should().BeTrue();
        ensuredResult.Error.Should().Be(originalError);
    }

    [Fact]
    public void Map_WithSuccess_ShouldMapValue()
    {
        // Arrange
        var result = Result.Success(42);

        // Act
        var mappedResult = result.Map(x => x.ToString());

        // Assert
        mappedResult.IsSuccess.Should().BeTrue();
        mappedResult.IsFailure.Should().BeFalse();
        mappedResult.Value.Should().Be("42");
    }

    [Fact]
    public void Map_WithFailure_ShouldPreserveError()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var result = Result.Failure<int>(error);

        // Act
        var mappedResult = result.Map(x => x.ToString());

        // Assert
        mappedResult.IsSuccess.Should().BeFalse();
        mappedResult.IsFailure.Should().BeTrue();
        mappedResult.Error.Should().Be(error);
    }

    [Fact]
    public async Task MapAsync_WithResultAndSuccess_ShouldMapValue()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var mappedValue = await result.MapAsync(async r =>
        {
            await Task.Delay(1); // Simulate async work
            return r.IsSuccess ? "success" : "failure";
        });

        // Assert
        mappedValue.Should().Be("success");
    }

    [Fact]
    public async Task MapAsync_WithResultAndFailure_ShouldMapValue()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var result = Result.Failure(error);

        // Act
        var mappedValue = await result.MapAsync(async r =>
        {
            await Task.Delay(1); // Simulate async work
            return r.IsSuccess ? "success" : "failure";
        });

        // Assert
        mappedValue.Should().Be("failure");
    }

    [Fact]
    public async Task MapAsync_WithResultTaskAndSuccess_ShouldMapValue()
    {
        // Arrange
        var resultTask = Task.FromResult(Result.Success());

        // Act
        var mappedValue = await resultTask.MapAsync(async r =>
        {
            await Task.Delay(1); // Simulate async work
            return r.IsSuccess ? "success" : "failure";
        });

        // Assert
        mappedValue.Should().Be("success");
    }

    [Fact]
    public async Task MapAsync_WithResultTaskAndFailure_ShouldMapValue()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var resultTask = Task.FromResult(Result.Failure(error));

        // Act
        var mappedValue = await resultTask.MapAsync(async r =>
        {
            await Task.Delay(1); // Simulate async work
            return r.IsSuccess ? "success" : "failure";
        });

        // Assert
        mappedValue.Should().Be("failure");
    }

    [Fact]
    public async Task MapAsync_WithGenericResultTaskAndSuccess_ShouldMapValue()
    {
        // Arrange
        var resultTask = Task.FromResult(Result.Success(42));

        // Act
        var mappedValue = await resultTask.MapAsync(async r =>
        {
            await Task.Delay(1); // Simulate async work
            return r.IsSuccess ? r.Value.ToString() : "failure";
        });

        // Assert
        mappedValue.Should().Be("42");
    }

    [Fact]
    public async Task MapAsync_WithGenericResultTaskAndFailure_ShouldMapValue()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var resultTask = Task.FromResult(Result.Failure<int>(error));

        // Act
        var mappedValue = await resultTask.MapAsync(async r =>
        {
            await Task.Delay(1); // Simulate async work
            return r.IsSuccess ? r.Value.ToString() : "failure";
        });

        // Assert
        mappedValue.Should().Be("failure");
    }

    [Fact]
    public async Task BindAsync_WithSuccess_ShouldBindToNewResult()
    {
        // Arrange
        var result = Result.Success(42);

        // Act
        var boundResult = await result.BindAsync(async x =>
        {
            await Task.Delay(1); // Simulate async work
            return Result.Success(x.ToString());
        });

        // Assert
        boundResult.IsSuccess.Should().BeTrue();
        boundResult.IsFailure.Should().BeFalse();
        boundResult.Value.Should().Be("42");
    }

    [Fact]
    public async Task BindAsync_WithFailure_ShouldPreserveError()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var result = Result.Failure<int>(error);

        // Act
        var boundResult = await result.BindAsync(async x =>
        {
            await Task.Delay(1); // Simulate async work
            return Result.Success(x.ToString());
        });

        // Assert
        boundResult.IsSuccess.Should().BeFalse();
        boundResult.IsFailure.Should().BeTrue();
        boundResult.Error.Should().Be(error);
    }
}
