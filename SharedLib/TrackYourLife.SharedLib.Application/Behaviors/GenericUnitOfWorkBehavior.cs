using MediatR;
using TrackYourLife.SharedLib.Domain.Repositories;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Application.Behaviors;

public abstract class GenericUnitOfWorkBehavior<TUnitOfWork, TRequest, TResponse>(
    TUnitOfWork unitOfWork
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
    where TUnitOfWork : IUnitOfWork
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (IsNotCommand())
        {
            return await next();
        }

        using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var response = await next();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return response;
    }

    private static bool IsNotCommand()
    {
        return !typeof(TRequest).Name.EndsWith("Command");
    }
}
