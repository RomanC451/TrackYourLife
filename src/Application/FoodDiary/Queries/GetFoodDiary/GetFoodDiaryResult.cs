using TrackYourLifeDotnet.Domain.FoodDiaries;

namespace TrackYourLifeDotnet.Application.FoodDiary.Queries.GetFoodDiary;

public sealed record GetFoodDiaryResult(
    List<FoodDiaryEntryDto> Breakfast,
    List<FoodDiaryEntryDto> Lunch,
    List<FoodDiaryEntryDto> Dinner,
    List<FoodDiaryEntryDto> Snacks
);
