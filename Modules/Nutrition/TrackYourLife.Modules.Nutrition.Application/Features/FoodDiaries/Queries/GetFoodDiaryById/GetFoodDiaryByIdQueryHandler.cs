using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Queries.GetFoodDiaryById;

internal sealed class GetFoodDiaryByIdQueryHandler(IFoodDiaryQuery foodDiaryQuery)
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
