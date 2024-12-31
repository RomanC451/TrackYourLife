using MediatR;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;

public interface ICommand : IRequest<Result>, IUsersRequest { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IUsersRequest { }
