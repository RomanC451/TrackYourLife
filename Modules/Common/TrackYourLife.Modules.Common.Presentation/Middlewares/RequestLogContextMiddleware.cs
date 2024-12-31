using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace TrackYourLife.Modules.Common.Presentation.Middlewares;

public sealed class RequestLogContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
        {
            return _next(context);
        }
    }
}
