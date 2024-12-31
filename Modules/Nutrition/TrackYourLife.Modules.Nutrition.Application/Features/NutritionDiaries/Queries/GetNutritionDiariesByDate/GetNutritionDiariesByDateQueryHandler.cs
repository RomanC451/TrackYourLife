using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionDiariesByDate;

public sealed class GetNutritionDiariesByDateQueryHandler(
    IFoodDiaryQuery foodDiaryQuery,
    IRecipeDiaryQuery recipeDiaryQuery,
    IUserIdentifierProvider userIdentifierProvider
)
    : IQueryHandler<
        GetNutritionDiariesByDateQuery,
        (
            Dictionary<MealTypes, List<FoodDiaryReadModel>>,
            Dictionary<MealTypes, List<RecipeDiaryReadModel>>
        )
    >
{
    public async Task<
        Result<(
            Dictionary<MealTypes, List<FoodDiaryReadModel>>,
            Dictionary<MealTypes, List<RecipeDiaryReadModel>>
        )>
    > Handle(GetNutritionDiariesByDateQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<FoodDiaryReadModel> foodDiaryEntries = await foodDiaryQuery.GetByDateAsync(
            userIdentifierProvider.UserId,
            query.Day,
            cancellationToken
        );

        Dictionary<MealTypes, List<FoodDiaryReadModel>> foodDiariesDict = Enum.GetValues(
                typeof(MealTypes)
            )
            .Cast<MealTypes>()
            .ToDictionary(mealType => mealType, mealType => new List<FoodDiaryReadModel>());

        foreach (var entry in foodDiaryEntries)
        {
            foodDiariesDict[entry.MealType].Add(entry);
        }

        IEnumerable<RecipeDiaryReadModel> recipeDiaryEntries =
            await recipeDiaryQuery.GetByDateAsync(
                userIdentifierProvider.UserId,
                query.Day,
                cancellationToken
            );

        Dictionary<MealTypes, List<RecipeDiaryReadModel>> recipeDiariesDict = Enum.GetValues(
                typeof(MealTypes)
            )
            .Cast<MealTypes>()
            .ToDictionary(mealType => mealType, mealType => new List<RecipeDiaryReadModel>());

        foreach (var entry in recipeDiaryEntries)
        {
            recipeDiariesDict[entry.MealType].Add(entry);
        }

        return Result.Success((foodDiariesDict, recipeDiariesDict));
    }
}
