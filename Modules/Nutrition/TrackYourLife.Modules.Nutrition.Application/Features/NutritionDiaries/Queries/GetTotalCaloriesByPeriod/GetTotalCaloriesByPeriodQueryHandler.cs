using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetTotalCaloriesByPeriod;

/// <summary>
/// Handles the <see cref="GetTotalCaloriesByPeriodQuery"/> by calculating the total calories for a given period.
/// </summary>
/// <param name="foodDiaryQuery">The food diary query.</param>
/// <param name="userIdentifierProvider">The user identifier provider.</param>
public class GetTotalCaloriesByPeriodQueryHandler(
    IFoodDiaryQuery foodDiaryQuery,
    IRecipeDiaryQuery recipeDiaryQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetTotalCaloriesByPeriodQuery, int>
{
    public async Task<Result<int>> Handle(
        GetTotalCaloriesByPeriodQuery query,
        CancellationToken cancellationToken
    )
    {
        var foodDiaries = await foodDiaryQuery.GetByPeriodAsync(
            userIdentifierProvider.UserId,
            query.StartDate,
            query.EndDate,
            cancellationToken
        );

        var recipeDiaries = await recipeDiaryQuery.GetByPeriodAsync(
            userIdentifierProvider.UserId,
            query.StartDate,
            query.EndDate,
            cancellationToken
        );

        int totalCalories =
            foodDiaries.Sum(de =>
                (int)(
                    de.Food.NutritionalContents.Energy.Value
                    * de.ServingSize.NutritionMultiplier
                    * de.Quantity
                )
            )
            + recipeDiaries.Sum(de =>
                (int)(de.Recipe.NutritionalContents.Energy.Value * de.Quantity)
            );

        return Result.Success(totalCalories);
    }
}
