using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.NutritionDiaries.Models;

public class NutritionDiaryMappingsExtensionsTests
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
        dto.DiaryType.Should().Be(DiaryType.FoodDiary);
    }

    [Fact]
    public void ToDto_WithRecipeDiaryReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var recipeId = RecipeId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var servingSizeReadModel = new ServingSizeReadModel(
            servingSizeId,
            1.0f,
            "portion",
            125.0f,
            null
        );

        var recipeReadModel = new RecipeReadModel(recipeId, userId, "Test Recipe", 4, 500.0f, false)
        {
            Ingredients = [],
            NutritionalContents = new(),
            ServingSizes = [servingSizeReadModel],
        };

        var recipeDiaryReadModel = new RecipeDiaryReadModel(
            nutritionDiaryId,
            userId,
            servingSizeId,
            1.5f,
            MealTypes.Lunch,
            date,
            DateTime.UtcNow
        )
        {
            Recipe = recipeReadModel,
        };

        // Act
        var dto = recipeDiaryReadModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(nutritionDiaryId);
        dto.Quantity.Should().Be(1.5f);
        dto.MealType.Should().Be(MealTypes.Lunch);
        dto.Date.Should().Be(date);
        dto.DiaryType.Should().Be(DiaryType.RecipeDiary);
    }
}
