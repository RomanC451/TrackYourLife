using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetTotalCaloriesByPeriod;

/// <summary>
/// Handles the <see cref="GetNutritionTotalsByPeriodQuery"/> by calculating the total calories for a given period.
/// </summary>
/// <param name="foodDiaryQuery">The food diary query.</param>
/// <param name="userIdentifierProvider">The user identifier provider.</param>
public class GetNutritionTotalsByPeriodQueryHandler(
    IFoodDiaryQuery foodDiaryQuery,
    IRecipeDiaryQuery recipeDiaryQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetNutritionTotalsByPeriodQuery, NutritionalContent>
{
    public async Task<Result<NutritionalContent>> Handle(
        GetNutritionTotalsByPeriodQuery query,
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

        var foodDiaryOverview = foodDiaries.Aggregate(
            new NutritionalContent(),
            (acc, de) =>
            {
                acc.AddNutritionalValues(
                    de.Food.NutritionalContents.MultiplyNutritionalValues(
                        de.ServingSize.NutritionMultiplier * de.Quantity
                    )
                );
                return acc;
            }
        );

        var recipeDiaryOverview = recipeDiaries.Aggregate(
            new NutritionalContent(),
            (acc, de) =>
            {
                acc.AddNutritionalValues(
                    de.Recipe.NutritionalContents.MultiplyNutritionalValues(de.Quantity)
                );
                return acc;
            }
        );

        var finalOverview = foodDiaryOverview;

        finalOverview.AddNutritionalValues(recipeDiaryOverview);

        return Result.Success(finalOverview);
    }
}
