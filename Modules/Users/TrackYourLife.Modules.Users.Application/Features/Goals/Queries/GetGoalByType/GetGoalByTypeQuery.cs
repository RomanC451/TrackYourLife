using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetGoalByType;

public sealed record GetGoalByTypeQuery(GoalType Type, DateOnly Date) : IQuery<GoalReadModel>;
