using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
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

        // Act
        var foodHistory = FoodHistory.Create(id, userId, foodId).Value;

        // Assert
        foodHistory.Should().NotBeNull();
        foodHistory.Id.Should().Be(id);
        foodHistory.UserId.Should().Be(userId);
        foodHistory.FoodId.Should().Be(foodId);
        foodHistory.LastUsedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithInvalidId_ShouldFail()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();

        // Act
        var result = FoodHistory.Create(FoodHistoryId.Empty, userId, foodId);

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

        // Act
        var result = FoodHistory.Create(id, UserId.Empty, foodId);

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

        // Act
        var result = FoodHistory.Create(id, userId, FoodId.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("FoodId");
        result.Error.Code.Should().Contain(nameof(FoodHistory));
    }

    [Fact]
    public void LastUsedNow_ShouldUpdateLastUsedAt()
    {
        // Arrange
        var id = FoodHistoryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var foodHistory = FoodHistory.Create(id, userId, foodId).Value;
        var initialTime = foodHistory.LastUsedAt;

        // Act
        foodHistory.LastUsedNow();

        // Assert
        foodHistory.LastUsedAt.Should().BeAfter(initialTime);
        foodHistory.LastUsedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
