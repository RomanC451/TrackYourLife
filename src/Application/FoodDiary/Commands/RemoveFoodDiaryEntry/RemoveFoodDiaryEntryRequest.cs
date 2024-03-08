using TrackYourLifeDotnet.Domain.FoodDiaries;

namespace TrackYourLifeDotnet.Application.FoodDiary.Commands.RemoveFoodDiaryEntry;

public sealed record RemoveFoodDiaryEntryRequest(FoodDiaryEntryId FoodDiaryEntryId);
