using TrackYourLifeDotnet.Domain.Shared;
using MediatR;

namespace TrackYourLifeDotnet.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
