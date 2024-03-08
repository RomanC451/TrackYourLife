using System.ComponentModel.DataAnnotations;
using TrackYourLifeDotnet.Domain.FoodDiaries;

namespace TrackYourLifeDotnet.Application.FoodDiary.Commands.AddFoodDieryEntry;

public sealed record AddFoodDiaryEntryRequest(
    [Required] Guid? FoodId,
    [Required] MealTypes? MealType,
    [Required] Guid? ServingSizeId,
    [Required] float? Quantity,
    [Required] DateOnly? Date
);
