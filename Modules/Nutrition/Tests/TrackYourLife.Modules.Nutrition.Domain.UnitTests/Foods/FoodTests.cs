using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Foods;

public class FoodTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateFood()
    {
        // Arrange
        var id = FoodId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();

        var servingSize = ServingSize.Create(servingSizeId, 1, "ml", 100).Value;

        // Act
        var food = Food.Create(
            id,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSizeId, servingSize, 0).Value]
        ).Value;

        // Assert
        food.Should().NotBeNull();
        food.Id.Should().Be(id);
        food.Type.Should().Be("Food");
        food.BrandName.Should().Be("Kaufland");
        food.CountryCode.Should().Be("R0");
        food.Name.Should().Be("Lapte");
        food.NutritionalContents.Should().NotBeNull();
        food.FoodServingSizes.Should().NotBeEmpty();
        food.ApiId.Should().BeNull();
    }

    [Fact]
    public void Create_WithInvalidId_ShouldFail()
    {
        // Arrange
        var foodId = FoodId.Empty;
        var servingSizeId = ServingSizeId.NewId();

        var servingSize = ServingSize.Create(servingSizeId, 1, "ml", 100).Value;

        // Act
        var result = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(FoodId.NewId(), servingSizeId, servingSize, 0).Value]
        );

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidType_ShouldFail()
    {
        // Arrange
        var id = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();

        var servingSize = ServingSize.Create(servingSizeId, 1, "ml", 100).Value;

        // Act
        var result = Food.Create(
            id,
            string.Empty,
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(id, servingSizeId, servingSize, 0).Value]
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Type");
        result.Error.Code.Should().Contain(nameof(Food));
    }

    [Fact]
    public void Create_WithInvalidName_ShouldFail()
    {
        // Arrange
        var id = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();

        var servingSize = ServingSize.Create(servingSizeId, 1, "ml", 100).Value;

        // Act
        var result = Food.Create(
            id,
            "Food",
            "Kaufland",
            "R0",
            string.Empty,
            new NutritionalContent(),
            [FoodServingSize.Create(id, servingSizeId, servingSize, 0).Value]
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Name");
        result.Error.Code.Should().Contain(nameof(Food));
    }

    [Fact]
    public void Create_WithInvalidNutritionalContents_ShouldFail()
    {
        // Arrange
        var id = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();

        var servingSize = ServingSize.Create(servingSizeId, 1, "ml", 100).Value;

        // Act
        var result = Food.Create(
            id,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            null!,
            [FoodServingSize.Create(id, servingSizeId, servingSize, 0).Value]
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Null");
        result.Error.Code.Should().Contain("NutritionalContent");
        result.Error.Code.Should().Contain(nameof(Food));
    }

    [Fact]
    public void Create_WithInvalidFoodServingSizes_ShouldFail()
    {
        // Arrange
        var id = FoodId.NewId();


        // Act
        var result = Food.Create(
            id,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            []
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("FoodServingSizes");
        result.Error.Code.Should().Contain(nameof(Food));
    }
}
