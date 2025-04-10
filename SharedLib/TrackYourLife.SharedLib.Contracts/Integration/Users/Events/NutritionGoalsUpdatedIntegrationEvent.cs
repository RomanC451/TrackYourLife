using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Contracts.Integration.Users.Events;

public sealed record NutritionGoalUpdatedIntegrationEvent(
    UserId UserId,
    DateOnly StartDate,
    DateOnly EndDate,
    float Value,
    GoalType Type
);
