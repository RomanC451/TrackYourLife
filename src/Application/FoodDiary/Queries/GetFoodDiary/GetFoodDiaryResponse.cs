using TrackYourLifeDotnet.Domain.FoodDiaries;

namespace TrackYourLifeDotnet.Application.FoodDiary.Queries.GetFoodDiary;

public sealed record GetFoodDiaryResponse(
    List<FoodDiaryEntryDto> Breakfast,
    List<FoodDiaryEntryDto> Lunch,
    List<FoodDiaryEntryDto> Dinner,
    List<FoodDiaryEntryDto> Snacks
);
