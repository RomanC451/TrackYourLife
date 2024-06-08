using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.FoodDiaries;

namespace TrackYourLife.Common.Application.FoodDiaries.Queries.GetTotalCaloriesByPeriod;

public sealed record GetTotalCaloriesByPeriodQuery(string StartDate, string EndDate)
    : IQuery<TotalCaloriesResponse>;
