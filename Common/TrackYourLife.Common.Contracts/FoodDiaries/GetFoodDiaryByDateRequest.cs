using System.ComponentModel.DataAnnotations;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Contracts.FoodDiaries;

public sealed record GetFoodDiaryByDateRequest([Required] DateOnly Day);
