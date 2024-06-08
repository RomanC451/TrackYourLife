namespace TrackYourLife.Common.Contracts.Foods;

public sealed record GetFoodListRequest(
    string SearchParam,
    int Page,
    int? PageSize = 1
);
