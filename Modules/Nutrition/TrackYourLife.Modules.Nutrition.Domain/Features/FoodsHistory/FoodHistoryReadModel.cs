using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;

public sealed record FoodHistoryReadModel(
    FoodHistoryId Id,
    UserId UserId,
    FoodId FoodId,
    DateTime LastUsedAt
) : IReadModel<FoodHistoryId>;
