using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.FoodDiary.Queries.GetFoodDiary;

public sealed record GetFoodDiaryQuery(DateOnly Date) : IQuery<GetFoodDiaryResult>;
