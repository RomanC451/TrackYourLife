using TrackYourLife.Modules.Nutrition.Contracts.MappingsExtensions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.Foods.Models;

public class FoodMappingsExtensionsTests
{
    [Fact]
    public void ToDto_WithServingSizeReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var servingSizeId = ServingSizeId.NewId();
        var servingSizeReadModel = new ServingSizeReadModel(
            servingSizeId,
            1.5f,
            "cup",
            200.0f,
            null
        );

        // Act
        var dto = servingSizeReadModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(servingSizeId);
        dto.NutritionMultiplier.Should().Be(1.5f);
        dto.Unit.Should().Be("cup");
        dto.Value.Should().Be(200.0f);
    }

    [Fact]
    public void ToDto_WithFoodReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var servingSizeReadModel = new ServingSizeReadModel(
            servingSizeId,
            1.0f,
            "cup",
            100.0f,
            null
        );

        var foodServingSizeReadModel = new FoodServingSizeReadModel(foodId, servingSizeId, 0)
        {
            ServingSize = servingSizeReadModel,
        };

        var foodReadModel = new FoodReadModel(foodId, "Test Food", "Generic", "Test Brand", "US")
        {
            NutritionalContents = new(),
            FoodServingSizes = [foodServingSizeReadModel],
        };

        // Act
        var dto = foodReadModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(foodId);
        dto.Name.Should().Be("Test Food");
        dto.Type.Should().Be("Generic");
        dto.BrandName.Should().Be("Test Brand");
        dto.CountryCode.Should().Be("US");
        dto.ServingSizes.Should().HaveCount(1);
        dto.ServingSizes[0].Id.Should().Be(servingSizeId);
    }
}
