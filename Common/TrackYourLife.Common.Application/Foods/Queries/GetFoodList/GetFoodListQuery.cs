using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.Foods;

namespace TrackYourLife.Common.Application.Foods.Queries.GetFoodList;

public sealed record GetFoodListQuery(string SearchParam, int Page, int PageSize)
    : IQuery<FoodListResponse>;
