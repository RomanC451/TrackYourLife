using MediatR;
using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.SharedLib.Application.Behaviors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Core.Behaviors;

/// <summary>
/// Represents a mediatR behavior that handles the unit of work for trainings-related requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
internal sealed class TrainingsUnitOfWorkBehavior<TRequest, TResponse>(
    ITrainingsUnitOfWork unitOfWork,
    IPublisher publisher
) : GenericUnitOfWorkBehavior<ITrainingsUnitOfWork, TRequest, TResponse>(unitOfWork, publisher)
    where TRequest : class, ITrainingsRequest
    where TResponse : Result;
