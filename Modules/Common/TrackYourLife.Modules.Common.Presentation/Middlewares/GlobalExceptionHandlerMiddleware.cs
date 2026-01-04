using Serilog;
using TrackYourLife.Modules.Common.Application.Core;

namespace TrackYourLife.Modules.Common.Presentation.Middlewares;

public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly CommonFeatureFlags _featureManagement;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger logger,
        CommonFeatureFlags featureManagement
    )
    {
        _next = next;
        _logger = logger;
        _featureManagement = featureManagement;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_featureManagement.ExposeExceptions)
        {
            await _next(context);
            return;
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unhandled exception occurred while processing the request");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync("{\"error\":\"An internal server error occurred\"}");
        }
    }
}
