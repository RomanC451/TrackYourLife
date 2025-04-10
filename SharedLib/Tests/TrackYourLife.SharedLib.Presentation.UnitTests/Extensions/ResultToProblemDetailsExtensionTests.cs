using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.Extensions;
using Xunit;

namespace TrackYourLife.SharedLib.Presentation.UnitTests.Extensions;

public class ResultToProblemDetailsExtensionTests
{
    [Fact]
    public void ToBadRequestProblemDetails_WithValidationResult_ShouldReturnValidationError()
    {
        // Arrange
        var errors = new Error[]
        {
            new("ValidationError", "First error"),
            new("ValidationError", "Second error"),
        };
        var result = ValidationResult.WithErrors(errors);

        // Act
        var problemDetails = result.ToBadRequestProblemDetails();

        // Assert
        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be("Validation Error");
        problemDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
        problemDetails.Type.Should().Be(errors[0].Code);
        problemDetails.Extensions.Should().ContainKey("errors");
        problemDetails.Extensions["errors"].Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ToBadRequestProblemDetails_WithNonValidationResult_ShouldReturnBadRequest()
    {
        // Arrange
        var error = new Error("BadRequest", "Invalid request");
        var result = Result.Failure(error);

        // Act
        var problemDetails = result.ToBadRequestProblemDetails();

        // Assert
        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be("Bad Request");
        problemDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
        problemDetails.Detail.Should().Be(error.Message);
        problemDetails.Type.Should().Be(error.Code);
        problemDetails.Extensions.Should().NotContainKey("errors");
    }

    [Fact]
    public void ToNoFoundProblemDetails_ShouldReturnNotFound()
    {
        // Arrange
        var error = new Error("NotFound", "Resource not found");
        var result = Result.Failure(error);

        // Act
        var problemDetails = result.ToNoFoundProblemDetails();

        // Assert
        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be("Not Found");
        problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
        problemDetails.Detail.Should().Be(error.Message);
        problemDetails.Type.Should().Be(error.Code);
    }

    [Fact]
    public void ToForbiddenProblemDetails_ShouldReturnForbidden()
    {
        // Arrange
        var error = new Error("Forbidden", "Access denied");
        var result = Result.Failure(error);

        // Act
        var problemDetails = result.ToForbiddenProblemDetails();

        // Assert
        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be("Forbidden");
        problemDetails.Status.Should().Be(StatusCodes.Status403Forbidden);
        problemDetails.Detail.Should().Be(error.Message);
        problemDetails.Type.Should().Be(error.Code);
    }

    [Fact]
    public void ToActionResult_WithSuccess_ShouldReturnNoContent()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var actionResult = result.ToActionResult();

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<NoContent>();
    }

    [Fact]
    public void ToActionResult_WithForbiddenError_ShouldReturnForbidden()
    {
        // Arrange
        var error = new Error("Forbidden", "Access denied", 403);
        var result = Result.Failure(error);

        // Act
        var actionResult = result.ToActionResult();

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<ProblemHttpResult>();
        var problemResult = (ProblemHttpResult)actionResult;
        problemResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        problemResult.ProblemDetails.Detail.Should().Be(error.Message);
    }

    [Fact]
    public void ToActionResult_WithNotFoundError_ShouldReturnNotFound()
    {
        // Arrange
        var error = new Error("NotFound", "Resource not found", 404);
        var result = Result.Failure(error);

        // Act
        var actionResult = result.ToActionResult();

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<NotFound<ProblemDetails>>();
        var notFoundResult = (NotFound<ProblemDetails>)actionResult;
        notFoundResult.Value.Should().NotBeNull();
        notFoundResult.Value!.Detail.Should().Be(error.Message);
        notFoundResult.Value.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public void ToActionResult_WithBadRequestError_ShouldReturnBadRequest()
    {
        // Arrange
        var error = new Error("BadRequest", "Invalid request");
        var result = Result.Failure(error);

        // Act
        var actionResult = result.ToActionResult();

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<BadRequest<ProblemDetails>>();
        var badRequestResult = (BadRequest<ProblemDetails>)actionResult;
        badRequestResult.Value.Should().NotBeNull();
        badRequestResult.Value!.Detail.Should().Be(error.Message);
        badRequestResult.Value.Status.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public void ToActionResult_WithValueAndSuccess_ShouldReturnSuccessResponse()
    {
        // Arrange
        var value = "Test Value";
        var result = Result.Success(value);

        // Act
        var actionResult = result.ToActionResult(v => TypedResults.Ok(v));

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<Ok<string>>();
        var okResult = (Ok<string>)actionResult;
        okResult.Value.Should().Be(value);
    }

    [Fact]
    public async Task ToActionResultAsync_WithSuccess_ShouldReturnNoContent()
    {
        // Arrange
        var result = Result.Success();
        var taskResult = Task.FromResult(result);

        // Act
        var actionResult = await taskResult.ToActionResultAsync();

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ToActionResultAsync_WithValueAndSuccess_ShouldReturnSuccessResponse()
    {
        // Arrange
        var value = "Test Value";
        var result = Result.Success(value);
        var taskResult = Task.FromResult(result);

        // Act
        var actionResult = await taskResult.ToActionResultAsync(v => TypedResults.Ok(v));

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<Ok<string>>();
        var okResult = (Ok<string>)actionResult;
        okResult.Value.Should().Be(value);
    }
}
