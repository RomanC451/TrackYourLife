using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Queries.GetRecipeDiaryById;

public sealed class GetFoodDiaryByIdQueryHandler(IFoodDiaryQuery foodDiaryQuery)
    : IQueryHandler<GetFoodDiaryByIdQuery, FoodDiaryReadModel>
{
    public async Task<Result<FoodDiaryReadModel>> Handle(
        GetFoodDiaryByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var foodDiary = await foodDiaryQuery.GetByIdAsync(query.DiaryId, cancellationToken);

        if (foodDiary is null)
        {
            return Result.Failure<FoodDiaryReadModel>(FoodDiaryErrors.NotFound(query.DiaryId));
        }

        return Result.Success(foodDiary);
    }
}
