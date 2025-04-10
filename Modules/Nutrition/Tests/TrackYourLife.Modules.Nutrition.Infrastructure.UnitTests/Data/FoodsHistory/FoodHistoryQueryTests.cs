using System.Reflection;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodsHistory;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.FoodsHistory;

[Collection("NutritionRepositoryTests")]
public class FoodHistoryQueryTests : BaseRepositoryTests
{
    private FoodHistoryQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new FoodHistoryQuery(_readDbContext!);
    }

    [Fact]
    public async Task GetHistoryByUserAsync_WhenHistoriesExist_ShouldReturnOrderedHistories()
    {
        // Arrange
        var userId = UserId.NewId();
        var otherUserId = UserId.NewId();

        var servingSizes = new List<ServingSize>
        {
            ServingSizeFaker.Generate(),
            ServingSizeFaker.Generate(),
            ServingSizeFaker.Generate(),
        };

        await _writeDbContext.ServingSizes.AddRangeAsync(servingSizes);
        // await _writeDbContext.SaveChangesAsync();

        var foodIds = new List<FoodId> { FoodId.NewId(), FoodId.NewId(), FoodId.NewId() };

        // Create and save Food first
        var food1 = FoodFaker.Generate(
            id: foodIds[0],
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(0, foodId: foodIds[0], servingSize: servingSizes[0]),
            ]
        );
        var food2 = FoodFaker.Generate(
            id: foodIds[1],
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(1, foodId: foodIds[1], servingSize: servingSizes[1]),
            ]
        );
        var food3 = FoodFaker.Generate(
            id: foodIds[2],
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(2, foodId: foodIds[2], servingSize: servingSizes[2]),
            ]
        );
        await _writeDbContext.Foods.AddRangeAsync(food1, food2, food3);
        await _writeDbContext.SaveChangesAsync();

        // Create and save FoodHistories for userId with different dates
        var oldestDate = DateTime.UtcNow.AddDays(-5);
        var middleDate = DateTime.UtcNow.AddDays(-2);
        var newestDate = DateTime.UtcNow;

        var foodHistory1 = FoodHistory.Create(FoodHistoryId.NewId(), userId, food1.Id).Value;

        var foodHistory2 = FoodHistory.Create(FoodHistoryId.NewId(), userId, food2.Id).Value;

        var foodHistory3 = FoodHistory.Create(FoodHistoryId.NewId(), userId, food3.Id).Value;

        // Create and save FoodHistory for otherUserId
        var foodHistory4 = FoodHistory.Create(FoodHistoryId.NewId(), otherUserId, food1.Id).Value;

        // Use reflection to set the LastUsedAt property
        var lastUsedAtProperty = typeof(FoodHistory).GetProperty(
            "LastUsedAt",
            BindingFlags.Public | BindingFlags.Instance
        );
        lastUsedAtProperty?.SetValue(foodHistory1, middleDate);
        lastUsedAtProperty?.SetValue(foodHistory2, oldestDate);
        lastUsedAtProperty?.SetValue(foodHistory3, newestDate);
        lastUsedAtProperty?.SetValue(foodHistory4, DateTime.UtcNow.AddDays(-10));

        await _writeDbContext.FoodHistories.AddRangeAsync(
            foodHistory1,
            foodHistory2,
            foodHistory3,
            foodHistory4
        );
        await _writeDbContext.SaveChangesAsync();

        true.Should().BeTrue();

        try
        {
            // Act
            var result = await _sut.GetHistoryByUserAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(3);

            // Check that the results are ordered by LastUsedAt in descending order
            resultList[0].Id.Should().Be(foodHistory3.Id);
            resultList[0].LastUsedAt.Should().BeCloseTo(newestDate, new TimeSpan(0, 0, 1));

            resultList[1].Id.Should().Be(foodHistory1.Id);
            resultList[1].LastUsedAt.Should().BeCloseTo(middleDate, new TimeSpan(0, 0, 1));

            resultList[2].Id.Should().Be(foodHistory2.Id);
            resultList[2].LastUsedAt.Should().BeCloseTo(oldestDate, new TimeSpan(0, 0, 1));
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetHistoryByUserAsync_WhenNoHistoriesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();

        try
        {
            // Act
            var result = await _sut.GetHistoryByUserAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
