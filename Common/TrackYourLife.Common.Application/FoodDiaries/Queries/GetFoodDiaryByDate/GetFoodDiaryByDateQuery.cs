using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.FoodDiaries;

namespace TrackYourLife.Common.Application.FoodDiaries.Queries.GetFoodDiaryByDate;

public sealed record GetFoodDiaryByDateQuery(DateOnly Day) : IQuery<FoodDiaryEntryListResponse>;
