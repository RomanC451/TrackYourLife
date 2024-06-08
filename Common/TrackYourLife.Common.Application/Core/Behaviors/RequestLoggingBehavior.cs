

using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Application.Core.Behaviors;

internal sealed class RequestLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : class, IRequest<TResponse> where TResponse : Result
{
    private readonly ILogger<RequestLoggingBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        _logger.LogInformation("Processing requestt {RequestName}", requestName);

        TResponse result = await next();

        if (result.IsSuccess)
            _logger.LogInformation("Completed request {RequestName}", requestName);
        else
        {
            using (LogContext.PushProperty("Error", result.Error, true))
            {
                _logger.LogError("Completed request {RequestName} with error", requestName);
            }
        }

        return result;
    }
}
