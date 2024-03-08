using System.ComponentModel.DataAnnotations;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.FoodDiary.Queries.GetFoodDiary;

public sealed record GetFoodDiaryRequest([Required] string? Date) : IQuery<GetFoodDiaryResult>;
