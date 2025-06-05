using MediatR;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;

public interface ICommand : IRequest<Result>, ITrainingsRequest { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, ITrainingsRequest { }
