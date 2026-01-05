using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.FoodDiaries.Models;

public class FoodDiaryMappingsExtensionsTests
{
    [Fact]
    public void ToDto_WithFoodDiaryReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var foodReadModel = new FoodReadModel(foodId, "Test Food", "Generic", "Test Brand", "US")
        {
            NutritionalContents = new(),
        };

        var servingSizeReadModel = new ServingSizeReadModel(
            servingSizeId,
            1.5f,
            "cup",
            200.0f,
            null
        );

        var foodDiaryReadModel = new FoodDiaryReadModel(
            nutritionDiaryId,
            userId,
            2.0f,
            MealTypes.Breakfast,
            date,
            DateTime.UtcNow
        )
        {
            Food = foodReadModel,
            ServingSize = servingSizeReadModel,
        };

        // Act
        var dto = foodDiaryReadModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(nutritionDiaryId);
        dto.Quantity.Should().Be(2.0f);
        dto.MealType.Should().Be(MealTypes.Breakfast);
        dto.Date.Should().Be(date);
        dto.Food.Should().NotBeNull();
        dto.ServingSize.Should().NotBeNull();
        dto.ServingSize.Id.Should().Be(servingSizeId);
    }
}
