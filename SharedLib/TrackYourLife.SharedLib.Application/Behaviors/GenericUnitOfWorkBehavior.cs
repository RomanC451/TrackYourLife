using MediatR;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Repositories;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Application.Behaviors;

public abstract class GenericUnitOfWorkBehavior<TUnitOfWork, TRequest, TResponse>(
    TUnitOfWork unitOfWork,
    IPublisher publisher
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
        if (IsNotCommand(request))
        {
            return await next();
        }

        for (int retry = 0; retry < MaxRetryCount; retry++)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next();

                // Collect domain events before saving changes
                var domainEvents = unitOfWork.GetDirectDomainEvents();

                await unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                // Publish domain events after transaction is committed
                foreach (var domainEvent in domainEvents)
                {
                    await publisher.Publish(domainEvent, cancellationToken);
                }

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

    private static bool IsNotCommand(TRequest request)
    {
        return !request.GetType().Name.EndsWith("Command");
    }
}
