using Microsoft.AspNetCore.Http;
using TrackYourLife.Modules.Common.Presentation.Middlewares;

namespace TrackYourLife.Modules.Common.Presentation.UnitTests.Middlewares;

public class RequestLogContextMiddlewareTests : IDisposable
{
    private readonly RequestLogContextMiddleware sut;
    private readonly RequestDelegate next = Substitute.For<RequestDelegate>();
    private readonly HttpContext context = Substitute.For<HttpContext>();

    public RequestLogContextMiddlewareTests()
    {
        sut = new RequestLogContextMiddleware(next);
    }

    [Fact]
    public async Task InvokeAsync_WhenCalled_PushesCorrelationIdToLogContext()
    {
        // Arrange
        var traceIdentifier = "test-correlation-id";
        context.TraceIdentifier.Returns(traceIdentifier);

        // Act
        await sut.InvokeAsync(context);

        // Assert
        await next.Received(1).Invoke(context);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        next.ClearSubstitute();
        context.ClearSubstitute();
    }
}
