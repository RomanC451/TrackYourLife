using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.Recipes.Models;

public class RecipeMappingsExtensionsTests
{
    [Fact]
    public void ToDto_WithIngredientReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var ingredientId = IngredientId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();

        var foodReadModel = new FoodReadModel(foodId, "Test Food", "Generic", "Test Brand", "US")
        {
            NutritionalContents = new(),
            FoodServingSizes = [],
        };

        var servingSizeReadModel = new ServingSizeReadModel(
            servingSizeId,
            1.0f,
            "cup",
            100.0f,
            null
        );

        var ingredientReadModel = new IngredientReadModel(ingredientId, 2.0f, DateTime.UtcNow)
        {
            Food = foodReadModel,
            ServingSize = servingSizeReadModel,
        };

        // Act
        var dto = ingredientReadModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(ingredientId);
        dto.Quantity.Should().Be(2.0f);
        dto.Food.Should().NotBeNull();
        dto.ServingSize.Should().NotBeNull();
    }

    [Fact]
    public void ToDto_WithRecipeReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var userId = UserId.NewId();
        var servingSizeId = ServingSizeId.NewId();

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

        // Act
        var dto = recipeReadModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(recipeId);
        dto.Name.Should().Be("Test Recipe");
        dto.Portions.Should().Be(4);
        dto.Weight.Should().Be(500.0f);
        dto.Ingredients.Should().BeEmpty();
        dto.ServingSizes.Should().HaveCount(1);
    }
}
