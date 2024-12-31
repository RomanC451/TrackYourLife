using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Goals;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateGoal;

public sealed record UpdateGoalCommand(
    GoalId Id,
    GoalType Type,
    int Value,
    GoalPeriod PerPeriod,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool Force = false
) : ICommand;
