using Microsoft.AspNetCore.Http;
using Serilog;
using TrackYourLife.Modules.Common.Application.Core;
using TrackYourLife.Modules.Common.Presentation.Middlewares;

namespace TrackYourLife.Modules.Common.Presentation.UnitTests.Middlewares;

public class GlobalExceptionHandlerMiddlewareTests : IDisposable
{
    private readonly GlobalExceptionHandlerMiddleware sut;
    private readonly RequestDelegate next = Substitute.For<RequestDelegate>();
    private readonly ILogger logger = Substitute.For<ILogger>();
    private readonly CommonFeatureFlags featureFlags = new();
    private readonly HttpContext context = Substitute.For<HttpContext>();
    private readonly HttpResponse response = Substitute.For<HttpResponse>();
    private readonly Stream responseBody = new MemoryStream();

    public GlobalExceptionHandlerMiddlewareTests()
    {
        context.Response.Returns(response);
        response.Body.Returns(responseBody);
        sut = new GlobalExceptionHandlerMiddleware(next, logger, featureFlags);
    }

    [Fact]
    public async Task InvokeAsync_WhenExposeExceptionsIsTrue_ShouldCallNext()
    {
        // Arrange
        featureFlags.ExposeExceptions = true;
        next.Invoke(context).Returns(Task.CompletedTask);

        // Act
        await sut.InvokeAsync(context);

        // Assert
        await next.Received(1).Invoke(context);
    }

    [Fact]
    public async Task InvokeAsync_WhenExposeExceptionsIsFalseAndNoException_ShouldCallNext()
    {
        // Arrange
        featureFlags.ExposeExceptions = false;
        next.Invoke(context).Returns(Task.CompletedTask);

        // Act
        await sut.InvokeAsync(context);

        // Assert
        await next.Received(1).Invoke(context);
    }

    [Fact]
    public async Task InvokeAsync_WhenExposeExceptionsIsFalseAndExceptionOccurs_ShouldHandleException()
    {
        // Arrange
        featureFlags.ExposeExceptions = false;
        var exception = new Exception("Test exception");
        var httpContext = new DefaultHttpContext();
        next.Invoke(Arg.Any<HttpContext>()).Returns(Task.FromException(exception));

        // Act
        await sut.InvokeAsync(httpContext);

        // Assert
        await next.Received(1).Invoke(httpContext);
        logger
            .Received(1)
            .Error(exception, "An unhandled exception occurred while processing the request");
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        httpContext.Response.ContentType.Should().Be("application/json");
    }

    [Fact]
    public async Task InvokeAsync_WhenExposeExceptionsIsFalseAndExceptionOccurs_ShouldWriteErrorResponse()
    {
        // Arrange
        featureFlags.ExposeExceptions = false;
        var exception = new Exception("Test exception");
        var httpContext = new DefaultHttpContext();
        var responseBodyStream = new MemoryStream();
        httpContext.Response.Body = responseBodyStream;
        next.Invoke(Arg.Any<HttpContext>()).Returns(Task.FromException(exception));

        // Act
        await sut.InvokeAsync(httpContext);

        // Assert
        responseBodyStream.Position = 0;
        var reader = new StreamReader(responseBodyStream);
        var responseContent = await reader.ReadToEndAsync();
        responseContent.Should().Be("{\"error\":\"An internal server error occurred\"}");
    }

    [Fact]
    public async Task InvokeAsync_WhenExposeExceptionsIsFalseAndExceptionOccurs_ShouldNotPropagateException()
    {
        // Arrange
        featureFlags.ExposeExceptions = false;
        var exception = new Exception("Test exception");
        var httpContext = new DefaultHttpContext();
        next.Invoke(Arg.Any<HttpContext>()).Returns(Task.FromException(exception));

        // Act
        var act = async () => await sut.InvokeAsync(httpContext);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task InvokeAsync_WhenExposeExceptionsIsTrueAndExceptionOccurs_ShouldPropagateException()
    {
        // Arrange
        featureFlags.ExposeExceptions = true;
        var exception = new Exception("Test exception");
        next.Invoke(context).Returns(Task.FromException(exception));

        // Act
        var act = async () => await sut.InvokeAsync(context);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");
        // When ExposeExceptions is true, next is called and the method returns immediately.
        // If an exception occurs, it propagates without being caught.
        await next.Received(1).Invoke(context);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            next.ClearSubstitute();
            context.ClearSubstitute();
            logger.ClearSubstitute();
            response.ClearSubstitute();
            responseBody.Dispose();
        }
    }
}
