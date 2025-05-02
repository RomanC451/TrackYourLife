using MediatR;

namespace TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;

public interface ICommand : IRequest<Result>, INutritionRequest { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, INutritionRequest { }
