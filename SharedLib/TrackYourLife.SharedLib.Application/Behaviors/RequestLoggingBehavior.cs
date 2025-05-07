using MediatR;
using Serilog;
using Serilog.Context;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Application.Behaviors;

public sealed class RequestLoggingBehavior<TRequest, TResponse>(ILogger logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        string requestName = typeof(TRequest).Name;

        logger.Information("Processing requestt {RequestName}", requestName);

        TResponse result = await next();

        if (result.IsSuccess)
            logger.Information("Completed request {RequestName}", requestName);
        else
        {
            using (LogContext.PushProperty("Error", result.Error, true))
            {
                logger.Error("Completed request {RequestName} with error", requestName);
            }
        }

        return result;
    }
}
