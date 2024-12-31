using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;

public sealed record FoodDiaryCreatedDomainEvent(UserId UserId, FoodId FoodId) : IDomainEvent;
