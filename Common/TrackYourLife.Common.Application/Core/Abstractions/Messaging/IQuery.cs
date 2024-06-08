using MediatR;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Application.Core.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
