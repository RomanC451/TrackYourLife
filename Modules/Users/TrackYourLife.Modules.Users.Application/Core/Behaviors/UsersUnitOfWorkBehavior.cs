using MediatR;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.SharedLib.Application.Behaviors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Core.Behaviors;

internal sealed class UsersUnitOfWorkBehavior<TRequest, TResponse>(
    IUsersUnitOfWork unitOfWork,
    IPublisher publisher
) : GenericUnitOfWorkBehavior<IUsersUnitOfWork, TRequest, TResponse>(unitOfWork, publisher)
    where TRequest : class, IUsersRequest
    where TResponse : Result;
