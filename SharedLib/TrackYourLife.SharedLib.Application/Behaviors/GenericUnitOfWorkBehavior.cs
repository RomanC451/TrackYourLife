using MediatR;
using Microsoft.EntityFrameworkCore;
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
    private const int MaxRetryCount = 3;

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
        for (int retry = 0; retry < MaxRetryCount; retry++)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next();

                await unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return response;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken);

                if (retry == MaxRetryCount - 1)
                {
                    throw;
                }

                await unitOfWork.ReloadUpdatedEntitiesAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        throw new InvalidOperationException("Max retry count exceeded.");
    }

    private static bool IsNotCommand()
    {
        return !typeof(TRequest).Name.EndsWith("Command");
    }
}
