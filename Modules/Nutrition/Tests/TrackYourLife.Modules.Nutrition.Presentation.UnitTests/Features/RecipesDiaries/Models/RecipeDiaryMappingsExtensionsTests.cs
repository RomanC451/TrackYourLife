using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.RecipesDiaries.Models;

public class RecipeDiaryMappingsExtensionsTests
{
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
            2.0f,
            MealTypes.Breakfast,
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
        dto.Quantity.Should().Be(2.0f);
        dto.MealType.Should().Be(MealTypes.Breakfast);
        dto.Date.Should().Be(date);
        dto.Recipe.Should().NotBeNull();
        dto.ServingSize.Should().NotBeNull();
    }
}
