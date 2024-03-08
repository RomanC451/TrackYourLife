using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.Foods.Queries.GetFoodList;

public sealed record GetFoodListQuery(string SearchParam, int Page, int PageSize)
    : IQuery<GetFoodListResult>;
