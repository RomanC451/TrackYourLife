using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.ServingSizes;

namespace TrackYourLife.Common.Application.FoodDiaries.Commands.UpdateFoodDiaryEntry;

public sealed record UpdateFoodDiaryEntryCommand(
    FoodDiaryEntryId Id,
    float Quantity,
    ServingSizeId ServingSizeId,
    MealTypes MealType
) : ICommand;
