using MediatR;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;

public interface ICommand : IRequest<Result>, IYoutubeRequest { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IYoutubeRequest { }

