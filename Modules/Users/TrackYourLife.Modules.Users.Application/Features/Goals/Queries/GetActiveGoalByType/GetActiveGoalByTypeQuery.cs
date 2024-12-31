using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Goals;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetActiveGoalByType;

public sealed record GetActiveGoalByTypeQuery(GoalType Type) : IQuery<GoalReadModel>;
