using MediatR;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Reading.Application.Core.Abstraction.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>, IReadingRequest { }
