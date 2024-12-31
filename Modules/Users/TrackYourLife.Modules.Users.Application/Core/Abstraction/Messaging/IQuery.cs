using MediatR;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>, IUsersRequest { }
