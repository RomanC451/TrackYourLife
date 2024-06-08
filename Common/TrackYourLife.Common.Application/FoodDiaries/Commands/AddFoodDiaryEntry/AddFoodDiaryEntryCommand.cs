using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.Common;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Domain.ServingSizes;

namespace TrackYourLife.Common.Application.FoodDiaries.Commands.AddFoodDiaryEntry;

public sealed record AddFoodDiaryEntryCommand(
    FoodId FoodId,
    MealTypes MealType,
    ServingSizeId ServingSizeId,
    float Quantity,
    DateOnly EntryDate
) : ICommand<IdResponse>;
