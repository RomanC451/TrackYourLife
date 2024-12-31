using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Users;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery() : IQuery<UserReadModel>;
