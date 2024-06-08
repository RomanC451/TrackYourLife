using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.UserGoals;
using TrackYourLife.Common.Domain.UserGoals;
using TrackYourLife.Domain.Shared;

namespace TrackYourLife.Modules.Users.Application.UserGoals.Commands.AddUserGoal;

public sealed record AddUserGoalCommand(
    int Value,
    UserGoalType Type,
    UserGoalPerPeriod PerPeriod,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool Force = false
) : ICommand<AddUserGoalResponse>;
