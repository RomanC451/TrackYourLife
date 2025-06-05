using MediatR;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.SharedLib.Application.Behaviors;

namespace TrackYourLife.Modules.Nutrition.Application.Core.Behaviors;

/// <summary>
/// Represents a mediatR behavior that handles the unit of work for nutrition-related requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
internal sealed class NutritionUnitOfWorkBehavior<TRequest, TResponse>(
    INutritionUnitOfWork unitOfWork,
    IPublisher publisher
) : GenericUnitOfWorkBehavior<INutritionUnitOfWork, TRequest, TResponse>(unitOfWork, publisher)
    where TRequest : class, INutritionRequest
    where TResponse : Result;
