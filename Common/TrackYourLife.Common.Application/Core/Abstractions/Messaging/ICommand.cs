using MediatR;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Application.Core.Abstractions.Messaging;

public interface ICommand : IRequest<Result> { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
