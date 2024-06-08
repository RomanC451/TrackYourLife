namespace TrackYourLife.Common.Contracts.FoodDiaries;

public sealed record FoodDiaryEntryListResponse(
    IReadOnlyCollection<FoodDiaryEntryResponse> Breakfast,
    IReadOnlyCollection<FoodDiaryEntryResponse> Lunch,
    IReadOnlyCollection<FoodDiaryEntryResponse> Dinner,
    IReadOnlyCollection<FoodDiaryEntryResponse> Snacks
);
