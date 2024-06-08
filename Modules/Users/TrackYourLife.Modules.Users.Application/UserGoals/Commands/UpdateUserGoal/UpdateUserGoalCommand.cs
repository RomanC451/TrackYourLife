using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.UserGoals;

namespace TrackYourLife.Modules.Users.Application.UserGoals.Commands.UpdateUserGoal;

public sealed record UpdateUserGoalCommand(
    UserGoalId Id,
    UserGoalType Type,
    int Value,
    UserGoalPerPeriod PerPeriod,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool Force = false
) : ICommand;
