using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Users;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery() : IQuery<UserReadModel>;
