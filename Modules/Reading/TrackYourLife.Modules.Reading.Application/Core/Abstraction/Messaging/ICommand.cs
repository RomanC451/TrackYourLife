using MediatR;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Reading.Application.Core.Abstraction.Messaging;

public interface ICommand : IRequest<Result>, IReadingRequest { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IReadingRequest { }
