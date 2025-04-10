using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Features.FoodServingSizes;

public class FoodServingSizeTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateFoodServingSize()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var index = 0;

        // Act
        var foodServingSize = FoodServingSize.Create(foodId, servingSizeId, index).Value;

        // Assert
        foodServingSize.Should().NotBeNull();
        foodServingSize.FoodId.Should().Be(foodId);
        foodServingSize.ServingSizeId.Should().Be(servingSizeId);
        foodServingSize.Index.Should().Be(index);
    }

    [Fact]
    public void Create_WithEmptyFoodId_ShouldReturnFailure()
    {
        // Arrange
        var foodId = FoodId.Empty;
        var servingSizeId = ServingSizeId.NewId();
        var index = 0;

        // Act
        var result = FoodServingSize.Create(foodId, servingSizeId, index);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("FoodId");
        result.Error.Code.Should().Contain(nameof(FoodServingSize));
    }

    [Fact]
    public void Create_WithEmptyServingSizeId_ShouldReturnFailure()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.Empty;
        var index = 0;

        // Act
        var result = FoodServingSize.Create(foodId, servingSizeId, index);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("ServingSizeId");
        result.Error.Code.Should().Contain(nameof(FoodServingSize));
    }

    [Fact]
    public void Create_WithNegativeIndex_ShouldReturnFailure()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var index = -1;

        // Act
        var result = FoodServingSize.Create(foodId, servingSizeId, index);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotPositive");
        result.Error.Code.Should().Contain("Index");
        result.Error.Code.Should().Contain(nameof(FoodServingSize));
    }
}
