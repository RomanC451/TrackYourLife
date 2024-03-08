using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.FoodDiaries;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;

namespace TrackYourLifeDotnet.Application.FoodDiary.Commands.UpdateFoodDiaryEntry;

public sealed record UpdateFoodDiaryEntryCommand(
    FoodDiaryEntryId FoodDiaryEntryId,
    float Quantity,
    ServingSizeId ServingSizeId,
    MealTypes MealType
) : ICommand;
