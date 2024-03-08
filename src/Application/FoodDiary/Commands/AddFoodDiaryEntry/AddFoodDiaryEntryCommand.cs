using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.FoodDiaries;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;

namespace TrackYourLifeDotnet.Application.FoodDiary.Commands.AddFoodDieryEntry;

public sealed record AddFoodDiaryEntryCommand(
    FoodId FoodId,
    MealTypes MealType,
    ServingSizeId ServingSizeId,
    float Quantity,
    DateOnly Date
) : ICommand<AddFoodDiaryEntryResult>;
