using TrackYourLifeDotnet.Domain.Foods.Dtos;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Application.Foods.Queries.GetFoodList;

public sealed record GetFoodListResult(PagedList<FoodDto> FoodList);
