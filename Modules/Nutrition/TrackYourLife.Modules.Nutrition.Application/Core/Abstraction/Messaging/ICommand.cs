
using MediatR;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;

public interface ICommand : IRequest<Result>, INutritionRequest { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, INutritionRequest { }
