using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Queries.GetRecipeDiaryById;

public sealed class GetRecipeDiaryByIdQueryHandler(IRecipeDiaryQuery recipeDiaryQuery)
    : IQueryHandler<GetRecipeDiaryByIdQuery, RecipeDiaryReadModel>
{
    public async Task<Result<RecipeDiaryReadModel>> Handle(
        GetRecipeDiaryByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var recipeDiary = await recipeDiaryQuery.GetByIdAsync(query.DiaryId, cancellationToken);

        if (recipeDiary is null)
        {
            return Result.Failure<RecipeDiaryReadModel>(RecipeDiaryErrors.NotFound(query.DiaryId));
        }

        return Result.Success(recipeDiary);
    }
}
