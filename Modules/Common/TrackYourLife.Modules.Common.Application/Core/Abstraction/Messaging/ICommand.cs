using MediatR;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Application.Core.Abstraction.Messaging;

public interface ICommand : IRequest<Result>, ICommonRequest;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, ICommonRequest;
