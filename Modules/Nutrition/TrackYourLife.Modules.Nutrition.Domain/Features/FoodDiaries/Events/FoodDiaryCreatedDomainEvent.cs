using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;

public sealed record FoodDiaryCreatedDomainEvent(
    UserId UserId,
    FoodId FoodId,
    DateOnly Date,
    ServingSizeId ServingSizeId,
    float Quantity
) : IDomainEvent;
