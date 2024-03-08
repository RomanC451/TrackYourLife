using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.FoodDiaries;
using TrackYourLifeDotnet.Domain.FoodDiaries.Queries;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;
using TrackYourLifeDotnet.Domain.Users.Queries;
using MapsterMapper;

namespace TrackYourLifeDotnet.Application.FoodDiary.Queries.GetFoodDiary;

public sealed class GetFoodDiaryQueryHandler : IQueryHandler<GetFoodDiaryQuery, GetFoodDiaryResult>
{
    private readonly IAuthService _authService;

    public readonly IUserQuery _userQuery;

    private readonly IFoodDiaryQuery _foodDiaryQuery;

    private readonly IMapper _mapper;

    public GetFoodDiaryQueryHandler(
        IFoodDiaryQuery foodDiaryQuery,
        IUserQuery userQuery,
        IAuthService authService,
        IMapper mapper
    )
    {
        _foodDiaryQuery = foodDiaryQuery;
        _userQuery = userQuery;
        _authService = authService;
        _mapper = mapper;
    }

    public async Task<Result<GetFoodDiaryResult>> Handle(
        GetFoodDiaryQuery request,
        CancellationToken cancellationToken
    )
    {
        var userIdResult = _authService.GetUserIdFromJwtToken();
        if (userIdResult.IsFailure)
        {
            return Result.Failure<GetFoodDiaryResult>(userIdResult.Error);
        }

        var userId = new UserId(userIdResult.Value);

        if (!await _userQuery.UserExistsAsync(userId, cancellationToken))
        {
            return Result.Failure<GetFoodDiaryResult>(DomainErrors.User.NotFound(userId));
        }

        List<FoodDiaryEntry> foodDiaryEntries = await _foodDiaryQuery.GetDailyFoodDiaryQuery(
            userId,
            request.Date,
            cancellationToken
        );

        List<FoodDiaryEntryDto> foodDiaryDtos = foodDiaryEntries
            .Select(_mapper.Map<FoodDiaryEntryDto>)
            .ToList();

        List<FoodDiaryEntryDto> breakfastDiary = foodDiaryDtos
            .Where(entry => entry.MealType == MealTypes.Breakfast)
            .ToList();

        List<FoodDiaryEntryDto> lunchDiary = foodDiaryDtos
            .Where(entry => entry.MealType == MealTypes.Lunch)
            .ToList();

        List<FoodDiaryEntryDto> dinnerDiary = foodDiaryDtos
            .Where(entry => entry.MealType == MealTypes.Dinner)
            .ToList();

        List<FoodDiaryEntryDto> snackDiary = foodDiaryDtos
            .Where(entry => entry.MealType == MealTypes.Snacks)
            .ToList();

        var response = new GetFoodDiaryResult(breakfastDiary, lunchDiary, dinnerDiary, snackDiary);

        return Result.Success(response);
    }
}
