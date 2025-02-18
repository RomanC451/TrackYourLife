using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Contracts.Integration.Users;

public sealed record GetNutritionGoalsByUserIdRequest(UserId UserId, DateOnly Date);

public sealed record GetNutritionGoalsByUserIdResponse(
    UserId UserId,
    DateOnly Date,
    float CaloriesGoal,
    float CarbohydratesGoal,
    float FatGoal,
    float ProteinGoal
);

public sealed record GetNutritionGoalsByUserIdErrorResponse(List<Error> Errors);
