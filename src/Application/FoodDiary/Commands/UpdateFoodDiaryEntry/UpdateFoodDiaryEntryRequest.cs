using System.ComponentModel.DataAnnotations;
using TrackYourLifeDotnet.Domain.FoodDiaries;

namespace TrackYourLifeDotnet.Application.FoodDiary.Commands.UpdateFoodDiaryEntry;

public sealed record UpdateFoodDiaryEntryRequest(
    [Required] Guid? FoodDiaryEntryId,
    [Required] float? Quantity,
    [Required] Guid? ServingSizeId,
    [Required] MealTypes? MealType
);
