using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Contracts.Goals;
using TrackYourLife.Modules.Users.Domain.Goals;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.AddGoal;

public sealed record AddGoalCommand(
    int Value,
    GoalType Type,
    GoalPeriod PerPeriod,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool Force = false
) : ICommand<GoalId>;
