using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.UserGoals;
using TrackYourLife.Common.Domain.UserGoals;

namespace TrackYourLife.Modules.Users.Application.UserGoals.Queries.GetActiveUserGoalByType;

public sealed record GetActiveUserGoalByTypeQuery(UserGoalType Type) : IQuery<UserGoalResponse>;
