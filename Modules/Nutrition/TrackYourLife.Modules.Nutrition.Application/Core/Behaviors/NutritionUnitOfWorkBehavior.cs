using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.SharedLib.Application.Behaviors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.Core.Behaviors;

/// <summary>
/// Represents a mediatR behavior that handles the unit of work for nutrition-related requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
internal sealed class NutritionUnitOfWorkBehavior<TRequest, TResponse>(
    INutritionUnitOfWork unitOfWork
) : GenericUnitOfWorkBehavior<INutritionUnitOfWork, TRequest, TResponse>(unitOfWork)
    where TRequest : class, INutritionRequest
    where TResponse : Result;
