using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.FoodServingSizes;

public class FoodServingSizeTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateFoodServingSize()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var servingSize = ServingSize.Create(servingSizeId, 1, "ml", 100).Value;
        var index = 0;

        // Act
        var foodServingSize = FoodServingSize
            .Create(foodId, servingSizeId, servingSize, index)
            .Value;

        // Assert
        foodServingSize.Should().NotBeNull();
        foodServingSize.FoodId.Should().Be(foodId);
        foodServingSize.ServingSizeId.Should().Be(servingSizeId);
        foodServingSize.ServingSize.Should().Be(servingSize);
        foodServingSize.Index.Should().Be(index);
    }

    [Fact]
    public void Create_WithEmptyFoodId_ShouldReturnFailure()
    {
        // Arrange
        var foodId = FoodId.Empty;
        var servingSizeId = ServingSizeId.NewId();
        var servingSize = ServingSize.Create(servingSizeId, 1, "ml", 100).Value;
        var index = 0;

        // Act
        var result = FoodServingSize.Create(foodId, servingSizeId, servingSize, index);

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
        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;
        var index = 0;

        // Act
        var result = FoodServingSize.Create(foodId, servingSizeId, servingSize, index);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("ServingSizeId");
        result.Error.Code.Should().Contain(nameof(FoodServingSize));
    }

    [Fact]
    public void Create_WithNullServingSize_ShouldReturnFailure()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var servingSize = (ServingSize)null!;
        var index = 0;

        // Act
        var result = FoodServingSize.Create(foodId, servingSizeId, servingSize, index);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Null");
        result.Error.Code.Should().Contain("ServingSize");
        result.Error.Code.Should().Contain(nameof(FoodServingSize));
    }

    [Fact]
    public void Create_WithNegativeIndex_ShouldReturnFailure()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var servingSize = ServingSize.Create(servingSizeId, 1, "ml", 100).Value;
        var index = -1;

        // Act
        var result = FoodServingSize.Create(foodId, servingSizeId, servingSize, index);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotPositive");
        result.Error.Code.Should().Contain("Index");
        result.Error.Code.Should().Contain(nameof(FoodServingSize));
    }
}
