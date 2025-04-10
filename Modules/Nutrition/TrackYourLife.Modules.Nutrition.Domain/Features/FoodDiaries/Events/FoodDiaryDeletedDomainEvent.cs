using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;

public sealed record FoodDiaryDeletedDomainEvent(
    UserId UserId,
    FoodId FoodId,
    ServingSizeId ServingSizeId,
    DateOnly Date,
    float Quantity
) : IDirectDomainEvent;
