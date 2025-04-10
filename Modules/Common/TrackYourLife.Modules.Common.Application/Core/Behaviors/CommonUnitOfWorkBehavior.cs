using MediatR;
using TrackYourLife.Modules.Common.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Common.Domain.Core;
using TrackYourLife.SharedLib.Application.Behaviors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Application.Core.Behaviors;

/// <summary>
/// Represents a mediatR behavior that handles the unit of work for common related requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
internal sealed class CommonUnitOfWorkBehavior<TRequest, TResponse>(
    ICommonUnitOfWork unitOfWork,
    IPublisher publisher
) : GenericUnitOfWorkBehavior<ICommonUnitOfWork, TRequest, TResponse>(unitOfWork, publisher)
    where TRequest : class, ICommonRequest
    where TResponse : Result;
