using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Features.FoodsHistory;

public class FoodHistoryTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateFoodHistory()
    {
        // Arrange
        var id = FoodHistoryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 2.5f;

        // Act
        var foodHistory = FoodHistory.Create(id, userId, foodId, servingSizeId, quantity).Value;

        // Assert
        foodHistory.Should().NotBeNull();
        foodHistory.Id.Should().Be(id);
        foodHistory.UserId.Should().Be(userId);
        foodHistory.FoodId.Should().Be(foodId);
        foodHistory.LastServingSizeUsedId.Should().Be(servingSizeId);
        foodHistory.LastQuantityUsed.Should().Be(quantity);
        foodHistory.LastUsedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithInvalidId_ShouldFail()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 1.0f;

        // Act
        var result = FoodHistory.Create(FoodHistoryId.Empty, userId, foodId, servingSizeId, quantity);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Id");
        result.Error.Code.Should().Contain(nameof(FoodHistory));
    }

    [Fact]
    public void Create_WithInvalidUserId_ShouldFail()
    {
        // Arrange
        var id = FoodHistoryId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 1.0f;

        // Act
        var result = FoodHistory.Create(id, UserId.Empty, foodId, servingSizeId, quantity);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("UserId");
        result.Error.Code.Should().Contain(nameof(FoodHistory));
    }

    [Fact]
    public void Create_WithInvalidFoodId_ShouldFail()
    {
        // Arrange
        var id = FoodHistoryId.NewId();
        var userId = UserId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 1.0f;

        // Act
        var result = FoodHistory.Create(id, userId, FoodId.Empty, servingSizeId, quantity);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("FoodId");
        result.Error.Code.Should().Contain(nameof(FoodHistory));
    }

    [Fact]
    public void LastUsedNow_ShouldUpdateLastUsedAtAndHistoryData()
    {
        // Arrange
        var id = FoodHistoryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 1.0f;
        var foodHistory = FoodHistory.Create(id, userId, foodId, servingSizeId, quantity).Value;
        var initialTime = foodHistory.LastUsedAt;
        var newServingSizeId = ServingSizeId.NewId();
        var newQuantity = 3.0f;

        // Act
        foodHistory.LastUsedNow(newServingSizeId, newQuantity);

        // Assert
        foodHistory.LastUsedAt.Should().BeAfter(initialTime);
        foodHistory.LastUsedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        foodHistory.LastServingSizeUsedId.Should().Be(newServingSizeId);
        foodHistory.LastQuantityUsed.Should().Be(newQuantity);
    }

    [Fact]
    public void Create_WithInvalidServingSizeId_ShouldFail()
    {
        // Arrange
        var id = FoodHistoryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var quantity = 1.0f;

        // Act
        var result = FoodHistory.Create(id, userId, foodId, ServingSizeId.Empty, quantity);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("LastServingSizeUsedId");
        result.Error.Code.Should().Contain(nameof(FoodHistory));
    }

    [Fact]
    public void Create_WithInvalidQuantity_ShouldFail()
    {
        // Arrange
        var id = FoodHistoryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();

        // Act
        var result = FoodHistory.Create(id, userId, foodId, servingSizeId, 0f);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotPositive");
        result.Error.Code.Should().Contain("LastQuantityUsed");
        result.Error.Code.Should().Contain(nameof(FoodHistory));
    }
}
