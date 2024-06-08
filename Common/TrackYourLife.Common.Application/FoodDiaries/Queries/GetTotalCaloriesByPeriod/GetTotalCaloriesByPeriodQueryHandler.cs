using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.FoodDiaries;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Application.FoodDiaries.Queries.GetTotalCaloriesByPeriod;

public class GetTotalCaloriesByPeriodQueryHandler
    : IQueryHandler<GetTotalCaloriesByPeriodQuery, TotalCaloriesResponse>
{
    readonly IFoodDiaryQuery _foodDiaryQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetTotalCaloriesByPeriodQueryHandler(
        IFoodDiaryQuery foodDiaryQuery,
        IUserIdentifierProvider userIdentifierProvider
    )
    {
        _foodDiaryQuery = foodDiaryQuery;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<Result<TotalCaloriesResponse>> Handle(
        GetTotalCaloriesByPeriodQuery query,
        CancellationToken cancellationToken
    )
    {
        int totalCalories = await _foodDiaryQuery.GetTotalCaloriesByPeriodQuery(
            _userIdentifierProvider.UserId,
            DateOnly.Parse(query.StartDate),
            DateOnly.Parse(query.EndDate),
            cancellationToken
        );

        return Result.Success(new TotalCaloriesResponse(totalCalories));
    }
}
