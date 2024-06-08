using MapsterMapper;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Users.Queries;
using TrackYourLife.Common.Contracts.FoodDiaries;

namespace TrackYourLife.Common.Application.FoodDiaries.Queries.GetFoodDiaryByDate;

public sealed class GetFoodDiaryByDateQueryHandler
    : IQueryHandler<GetFoodDiaryByDateQuery, FoodDiaryEntryListResponse>
{
    public readonly IUserQuery _userQuery;

    private readonly IFoodDiaryQuery _foodDiaryQuery;

    private readonly IMapper _mapper;

    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetFoodDiaryByDateQueryHandler(
        IFoodDiaryQuery foodDiaryQuery,
        IUserQuery userQuery,
        IMapper mapper,
        IUserIdentifierProvider userIdentifierProvider
    )
    {
        _foodDiaryQuery = foodDiaryQuery;
        _userQuery = userQuery;
        _mapper = mapper;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<Result<FoodDiaryEntryListResponse>> Handle(
        GetFoodDiaryByDateQuery query,
        CancellationToken cancellationToken
    )
    {
        List<FoodDiaryEntry> foodDiaryEntries = await _foodDiaryQuery.GetFoodDiaryByDateQuery(
            _userIdentifierProvider.UserId,
            query.Day,
            cancellationToken
        );

        List<FoodDiaryEntryResponse> foodDiaryDtos = foodDiaryEntries
            .Select(_mapper.Map<FoodDiaryEntryResponse>)
            .ToList();

        List<FoodDiaryEntryResponse> breakfastDiary = foodDiaryDtos
            .Where(entry => entry.MealType == MealTypes.Breakfast)
            .ToList();

        List<FoodDiaryEntryResponse> lunchDiary = foodDiaryDtos
            .Where(entry => entry.MealType == MealTypes.Lunch)
            .ToList();

        List<FoodDiaryEntryResponse> dinnerDiary = foodDiaryDtos
            .Where(entry => entry.MealType == MealTypes.Dinner)
            .ToList();

        List<FoodDiaryEntryResponse> snackDiary = foodDiaryDtos
            .Where(entry => entry.MealType == MealTypes.Snacks)
            .ToList();

        var response = new FoodDiaryEntryListResponse(
            breakfastDiary,
            lunchDiary,
            dinnerDiary,
            snackDiary
        );

        return Result.Success(response);
    }
}
