using TrackYourLife.Common.Contracts.Common;

namespace TrackYourLife.Common.Contracts.Foods;

public sealed record FoodListResponse(PagedList<FoodResponse> List);
