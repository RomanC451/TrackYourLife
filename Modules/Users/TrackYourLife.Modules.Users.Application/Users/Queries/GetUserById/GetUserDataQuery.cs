using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.Users;

namespace TrackYourLife.Modules.Users.Application.Users.Queries.GetUserById;

public sealed record GetUserDataQuery() : IQuery<UserResponse>;
