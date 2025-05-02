using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Contracts.Dtos;

public sealed record GoalDto(
    GoalId Id,
    GoalType Type,
    int Value,
    GoalPeriod PerPeriod,
    DateOnly StartDate,
    DateOnly EndDate
);
