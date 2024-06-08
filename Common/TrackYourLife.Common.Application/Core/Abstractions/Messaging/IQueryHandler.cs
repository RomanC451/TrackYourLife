using MediatR;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Application.Core.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{ }
