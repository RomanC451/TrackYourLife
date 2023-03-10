using TrackYourLifeDotnet.Domain.Shared;
using MediatR;

namespace TrackYourLifeDotnet.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse> { }
