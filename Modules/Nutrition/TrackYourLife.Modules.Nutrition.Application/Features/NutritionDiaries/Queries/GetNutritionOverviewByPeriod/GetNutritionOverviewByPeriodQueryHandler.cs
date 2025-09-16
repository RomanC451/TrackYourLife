using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionOverviewByPeriod;

/// <summary>
/// Handles the <see cref="GetNutritionOverviewByPeriodQuery"/> by calculating the total calories for a given period.
/// </summary>
/// <param name="foodDiaryQuery">The food diary query.</param>
/// <param name="userIdentifierProvider">The user identifier provider.</param>
internal sealed class GetNutritionOverviewByPeriodQueryHandler(
    IFoodDiaryQuery foodDiaryQuery,
    IRecipeDiaryQuery recipeDiaryQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetNutritionOverviewByPeriodQuery, NutritionalContent>
{
    public async Task<Result<NutritionalContent>> Handle(
        GetNutritionOverviewByPeriodQuery query,
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
                var servingSize = de.Recipe.ServingSizes.FirstOrDefault(x =>
                    x.Id == de.ServingSizeId
                );

                if (servingSize is null)
                {
                    return acc; // Skip this entry if serving size not found
                }

                acc.AddNutritionalValues(
                    de.Recipe.NutritionalContents.MultiplyNutritionalValues(
                        servingSize.NutritionMultiplier * de.Quantity
                    )
                );
                return acc;
            }
        );

        var finalOverview = foodDiaryOverview;

        finalOverview.AddNutritionalValues(recipeDiaryOverview);

        return Result.Success(finalOverview);
    }
}
